import React, { useState, useEffect } from 'react';
import { Tag, CreateTagRequest, UpdateTagRequest } from '../types/tag';
import { tagService } from '../services/tagService';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from './ui';
import TagBadge from './ui/TagBadge';

interface TagManagerProps {
  isOpen: boolean;
  onClose: () => void;
}

const TagManager: React.FC<TagManagerProps> = ({ isOpen, onClose }) => {
  const [tags, setTags] = useState<Tag[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [editingTag, setEditingTag] = useState<Tag | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    color: '',
    description: ''
  });

  useEffect(() => {
    if (isOpen) {
      loadTags();
    }
  }, [isOpen]);

  const loadTags = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const fetchedTags = await tagService.getTags();
      setTags(fetchedTags);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load tags');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateTag = () => {
    if (isCreating || editingTag) {
      // Cancel current operation
      handleCancel();
    } else {
      // Start creating new tag
      setEditingTag(null);
      setIsCreating(true);
      setFormData({ name: '', color: '', description: '' });
    }
  };

  const handleEditTag = (tag: Tag) => {
    setEditingTag(tag);
    setIsCreating(false);
    setFormData({
      name: tag.name,
      color: tag.color || '',
      description: tag.description || ''
    });
  };

  const handleSaveTag = async () => {
    try {
      setIsLoading(true);
      setError(null);

      if (isCreating) {
        const createData: CreateTagRequest = {
          name: formData.name.trim(),
          color: formData.color || undefined,
          description: formData.description.trim() || undefined
        };
        
        const newTag = await tagService.createTag(createData);
        setTags(prev => [...prev, newTag]);
      } else if (editingTag) {
        const updateData: UpdateTagRequest = {
          name: formData.name.trim(),
          color: formData.color || undefined,
          description: formData.description.trim() || undefined
        };
        
        const updatedTag = await tagService.updateTag(editingTag.id, updateData);
        setTags(prev => prev.map(tag => tag.id === editingTag.id ? updatedTag : tag));
      }

      setEditingTag(null);
      setIsCreating(false);
      setFormData({ name: '', color: '', description: '' });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save tag');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteTag = async (tagId: number) => {
    if (!confirm('Are you sure you want to delete this tag? This will remove it from all tasks.')) {
      return;
    }

    try {
      setIsLoading(true);
      setError(null);
      await tagService.deleteTag(tagId);
      setTags(prev => prev.filter(tag => tag.id !== tagId));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete tag');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    setEditingTag(null);
    setIsCreating(false);
    setFormData({ name: '', color: '', description: '' });
    setError(null);
  };

  const generateRandomColor = (): string => {
    const colors = [
      '#3B82F6', '#10B981', '#8B5CF6', '#F59E0B', '#EF4444',
      '#06B6D4', '#84CC16', '#F97316', '#EC4899', '#6366F1'
    ];
    return colors[Math.floor(Math.random() * colors.length)];
  };

  const handleRandomColor = () => {
    setFormData(prev => ({ ...prev, color: generateRandomColor() }));
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalHeader title="Manage Tags" onClose={onClose} />

      <ModalBody>
        {error && (
          <div className="mb-4 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-300 rounded-md">
            {error}
          </div>
        )}

        {/* Create/Edit Form */}
        {(isCreating || editingTag) && (
          <div className="mb-6 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
            <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100 mb-4">
              {isCreating ? 'Create New Tag' : 'Edit Tag'}
            </h3>
            
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Tag Name *
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter tag name"
                  maxLength={50}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Color
                </label>
                <div className="flex gap-2">
                  <input
                    type="color"
                    value={formData.color || '#3B82F6'}
                    onChange={(e) => setFormData(prev => ({ ...prev, color: e.target.value }))}
                    className="w-12 h-10 border border-gray-300 dark:border-gray-600 rounded-lg cursor-pointer"
                  />
                  <button
                    type="button"
                    onClick={handleRandomColor}
                    className="px-3 py-2 text-sm bg-gray-200 dark:bg-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-300 dark:hover:bg-gray-500 transition-colors"
                  >
                    Random
                  </button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Description
                </label>
                <textarea
                  value={formData.description}
                  onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter tag description (optional)"
                  rows={3}
                  maxLength={200}
                />
              </div>

              {/* Preview */}
              {formData.name && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Preview
                  </label>
                  <TagBadge
                    tag={{
                      id: 0,
                      name: formData.name,
                      color: formData.color,
                      description: formData.description,
                      createdAt: '',
                      updatedAt: '',
                      createdByUserId: 0
                    }}
                    size="md"
                  />
                </div>
              )}
            </div>
          </div>
        )}

        {/* Tags List */}
        <div>
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100">
              Your Tags ({tags.length})
            </h3>
            <Button 
              onClick={handleCreateTag} 
              size="sm"
              variant={isCreating || editingTag ? "outline" : "primary"}
            >
              {isCreating ? "Cancel Create" : editingTag ? "Cancel Edit" : "Create Tag"}
            </Button>
          </div>

          {isLoading && (
            <div className="text-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
              <p className="mt-2 text-gray-600 dark:text-gray-400">Loading tags...</p>
            </div>
          )}

          {!isLoading && tags.length === 0 && (
            <div className="text-center py-8 text-gray-500 dark:text-gray-400">
              <p>No tags created yet.</p>
              <p className="text-sm mt-1">Create your first tag to get started!</p>
            </div>
          )}

          {!isLoading && tags.length > 0 && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {tags.map(tag => (
                <div
                  key={tag.id}
                  className="p-4 border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800"
                >
                  <div className="flex items-start justify-between mb-2">
                    <TagBadge tag={tag} size="md" />
                    <div className="flex gap-1">
                      <button
                        onClick={() => handleEditTag(tag)}
                        className="p-1 text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                        title="Edit tag"
                      >
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                        </svg>
                      </button>
                      <button
                        onClick={() => handleDeleteTag(tag.id)}
                        className="p-1 text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors"
                        title="Delete tag"
                      >
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                      </button>
                    </div>
                  </div>
                  
                  {tag.description && (
                    <p className="text-sm text-gray-600 dark:text-gray-400 mb-2">
                      {tag.description}
                    </p>
                  )}
                  
                  <div className="text-xs text-gray-500 dark:text-gray-500">
                    Created {new Date(tag.createdAt).toLocaleDateString()}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </ModalBody>

      <ModalFooter>
        <div className="flex justify-between w-full">
          <div>
            {(isCreating || editingTag) && (
              <Button variant="outline" onClick={handleCancel}>
                Cancel
              </Button>
            )}
          </div>
          <div className="flex gap-2">
            {(isCreating || editingTag) && (
              <Button
                onClick={handleSaveTag}
                loading={isLoading}
                disabled={!formData.name.trim()}
              >
                {isCreating ? 'Create Tag' : 'Save Changes'}
              </Button>
            )}
            <Button variant="outline" onClick={onClose}>
              Close
            </Button>
          </div>
        </div>
      </ModalFooter>
    </Modal>
  );
};

export default TagManager;
