import React, { useState, useEffect, useRef } from 'react';
import { Tag } from '../../types/tag';
import { tagService } from '../../services/tagService';
import TagBadge from './TagBadge';

export interface TagSelectorProps {
  selectedTags: Tag[];
  onTagsChange: (tags: Tag[]) => void;
  placeholder?: string;
  maxTags?: number;
  allowCreate?: boolean;
  className?: string;
  disabled?: boolean;
}

const TagSelector: React.FC<TagSelectorProps> = ({
  selectedTags,
  onTagsChange,
  placeholder = "Search or select tags...",
  maxTags,
  allowCreate = true,
  className = '',
  disabled = false
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const inputRef = useRef<HTMLInputElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Load available tags
  useEffect(() => {
    loadAvailableTags();
  }, []);

  // Search tags when search term changes
  useEffect(() => {
    if (searchTerm.trim()) {
      searchTags(searchTerm);
    } else {
      loadAvailableTags();
    }
  }, [searchTerm]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const loadAvailableTags = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const tags = await tagService.getTags();
      // Filter out already selected tags
      const filteredTags = tags.filter(tag => 
        !selectedTags.some(selected => selected.id === tag.id)
      );
      setAvailableTags(filteredTags);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load tags');
    } finally {
      setIsLoading(false);
    }
  };

  const searchTags = async (term: string) => {
    try {
      setIsLoading(true);
      setError(null);
      const tags = await tagService.searchTags(term);
      // Filter out already selected tags
      const filteredTags = tags.filter(tag => 
        !selectedTags.some(selected => selected.id === tag.id)
      );
      setAvailableTags(filteredTags);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to search tags');
    } finally {
      setIsLoading(false);
    }
  };

  const handleTagSelect = (tag: Tag) => {
    if (maxTags && selectedTags.length >= maxTags) {
      return;
    }
    
    onTagsChange([...selectedTags, tag]);
    setSearchTerm('');
    setIsOpen(false);
  };

  const handleTagRemove = (tagId: number) => {
    onTagsChange(selectedTags.filter(tag => tag.id !== tagId));
  };

  const handleCreateTag = async () => {
    if (!allowCreate || !searchTerm.trim()) return;
    
    try {
      setIsLoading(true);
      setError(null);
      
      const newTag = await tagService.createTag({
        name: searchTerm.trim(),
        color: generateRandomColor()
      });
      
      onTagsChange([...selectedTags, newTag]);
      setSearchTerm('');
      setIsOpen(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create tag');
    } finally {
      setIsLoading(false);
    }
  };

  const generateRandomColor = (): string => {
    const colors = [
      '#3B82F6', '#10B981', '#8B5CF6', '#F59E0B', '#EF4444',
      '#06B6D4', '#84CC16', '#F97316', '#EC4899', '#6366F1'
    ];
    return colors[Math.floor(Math.random() * colors.length)];
  };

  const handleInputFocus = () => {
    if (!disabled) {
      setIsOpen(true);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && searchTerm.trim()) {
      e.preventDefault();
      if (availableTags.length > 0) {
        handleTagSelect(availableTags[0]);
      } else if (allowCreate) {
        handleCreateTag();
      }
    } else if (e.key === 'Escape') {
      setIsOpen(false);
    }
  };

  const canCreateTag = allowCreate && 
    searchTerm.trim() && 
    !availableTags.some(tag => tag.name.toLowerCase() === searchTerm.toLowerCase());

  // Debug info
  console.log('TagSelector Debug:', {
    allowCreate,
    searchTerm: searchTerm.trim(),
    canCreateTag,
    availableTagsCount: availableTags.length
  });

  return (
    <div className={`relative ${className}`}>
      {/* Selected tags */}
      <div className="flex flex-wrap gap-2 mb-2">
        {selectedTags.map(tag => (
          <TagBadge
            key={tag.id}
            tag={tag}
            size="sm"
            removable
            onRemove={handleTagRemove}
          />
        ))}
      </div>

      {/* Input field */}
      <div className="relative">
        <input
          ref={inputRef}
          type="text"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onFocus={handleInputFocus}
          onKeyDown={handleKeyDown}
          placeholder={placeholder}
          disabled={disabled}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
        />
        
        {/* Dropdown arrow */}
        <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
          <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        </div>
      </div>

      {/* Dropdown */}
      {isOpen && (
        <div
          ref={dropdownRef}
          className="absolute z-50 w-full mt-1 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg max-h-60 overflow-y-auto"
        >
          {isLoading && (
            <div className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
              Loading...
            </div>
          )}

          {error && (
            <div className="px-3 py-2 text-sm text-red-600 dark:text-red-400">
              {error}
            </div>
          )}

          {!isLoading && !error && availableTags.length === 0 && !searchTerm.trim() && (
            <div className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
              No tags available
            </div>
          )}

          {!isLoading && !error && availableTags.length === 0 && searchTerm.trim() && (
            <div className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
              No tags found
            </div>
          )}

          {/* Available tags */}
          {availableTags.map(tag => (
            <button
              key={tag.id}
              onClick={() => handleTagSelect(tag)}
              className="w-full px-3 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 focus:bg-gray-100 dark:focus:bg-gray-700 focus:outline-none"
            >
              <TagBadge tag={tag} size="sm" />
            </button>
          ))}

          {/* Debug info */}
          <div className="px-3 py-1 text-xs text-gray-400 border-t border-gray-200 dark:border-gray-600">
            Debug: allowCreate={allowCreate.toString()}, searchTerm="{searchTerm.trim()}", canCreate={canCreateTag.toString()}
          </div>

          {/* Create new tag option */}
          {canCreateTag && (
            <button
              onClick={handleCreateTag}
              className="w-full px-3 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 focus:bg-gray-100 dark:focus:bg-gray-700 focus:outline-none border-t border-gray-200 dark:border-gray-600"
            >
              <div className="flex items-center gap-2">
                <svg className="w-4 h-4 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                <span className="text-sm text-green-600 dark:text-green-400">
                  Create "{searchTerm.trim()}"
                </span>
              </div>
            </button>
          )}
        </div>
      )}
    </div>
  );
};

export default TagSelector;
