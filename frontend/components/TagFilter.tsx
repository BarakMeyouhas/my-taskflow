import React, { useState, useEffect } from 'react';
import { Tag } from '../types/tag';
import { tagService } from '../services/tagService';
import TagBadge from './ui/TagBadge';

interface TagFilterProps {
  selectedTags: Tag[];
  onTagsChange: (tags: Tag[]) => void;
  className?: string;
}

const TagFilter: React.FC<TagFilterProps> = ({
  selectedTags,
  onTagsChange,
  className = ''
}) => {
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  // Load available tags
  useEffect(() => {
    loadTags();
  }, []);

  const loadTags = async () => {
    try {
      setIsLoading(true);
      const tags = await tagService.getTags();
      setAvailableTags(tags);
    } catch (error) {
      console.error('Failed to load tags:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleTagToggle = (tag: Tag) => {
    const isSelected = selectedTags.some(t => t.id === tag.id);
    
    if (isSelected) {
      // Remove tag from selection
      onTagsChange(selectedTags.filter(t => t.id !== tag.id));
    } else {
      // Add tag to selection
      onTagsChange([...selectedTags, tag]);
    }
  };

  const handleRemoveTag = (tagId: number) => {
    onTagsChange(selectedTags.filter(t => t.id !== tagId));
  };

  const filteredTags = availableTags.filter(tag =>
    tag.name.toLowerCase().includes(searchTerm.toLowerCase()) &&
    !selectedTags.some(selected => selected.id === tag.id)
  );

  const clearAllTags = () => {
    onTagsChange([]);
  };

  return (
    <div className={`relative ${className}`}>
      {/* Selected Tags Display */}
      {selectedTags.length > 0 && (
        <div className="mb-2">
          <div className="flex items-center justify-between mb-1">
            <span className="text-sm text-gray-600 dark:text-gray-400">
              Filtered by tags ({selectedTags.length}):
            </span>
            <button
              onClick={clearAllTags}
              className="text-xs text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            >
              Clear all
            </button>
          </div>
          <div className="flex flex-wrap gap-1">
            {selectedTags.map(tag => (
              <TagBadge
                key={tag.id}
                tag={tag}
                removable
                onRemove={() => handleRemoveTag(tag.id)}
                size="sm"
              />
            ))}
          </div>
        </div>
      )}

      {/* Tag Selector */}
      <div className="relative">
        <button
          onClick={() => setIsOpen(!isOpen)}
          className="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-500 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-600 hover:bg-gray-50 dark:hover:bg-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
        >
          <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
          </svg>
          Filter by Tags
          {selectedTags.length > 0 && (
            <span className="ml-2 bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300 text-xs font-medium px-2 py-0.5 rounded-full">
              {selectedTags.length}
            </span>
          )}
        </button>

        {/* Dropdown */}
        {isOpen && (
          <div className="absolute z-10 mt-1 w-80 bg-white dark:bg-gray-700 rounded-md shadow-lg border border-gray-200 dark:border-gray-600">
            <div className="p-3">
              {/* Search Input */}
              <div className="relative mb-3">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg className="h-4 w-4 text-gray-400 dark:text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                </div>
                <input
                  type="text"
                  placeholder="Search tags..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="block w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-500 rounded-md leading-5 bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm"
                />
              </div>

              {/* Tags List */}
              <div className="max-h-48 overflow-y-auto">
                {isLoading ? (
                  <div className="text-center py-4">
                    <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mx-auto"></div>
                    <p className="text-sm text-gray-500 dark:text-gray-400 mt-2">Loading tags...</p>
                  </div>
                ) : filteredTags.length > 0 ? (
                  <div className="space-y-1">
                    {filteredTags.map(tag => (
                      <button
                        key={tag.id}
                        onClick={() => handleTagToggle(tag)}
                        className="w-full text-left px-3 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 rounded-md transition-colors"
                      >
                        <TagBadge tag={tag} size="sm" />
                      </button>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-4">
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                      {searchTerm ? 'No tags found matching your search' : 'No tags available'}
                    </p>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Click outside to close */}
      {isOpen && (
        <div
          className="fixed inset-0 z-5"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  );
};

export default TagFilter;
