import React, { useState } from 'react';
import { Tag } from '../types/tag';

export interface FilterOptions {
  status: ('ToDo' | 'InProgress' | 'Done')[];
  priority: ('Low' | 'Medium' | 'High')[];
  tags: Tag[];
  dueDateRange: {
    start?: string;
    end?: string;
  };
  createdDateRange: {
    start?: string;
    end?: string;
  };
}

interface TaskFilterProps {
  filters: FilterOptions;
  onFiltersChange: (filters: FilterOptions) => void;
  availableTags: Tag[];
  className?: string;
}

const TaskFilter: React.FC<TaskFilterProps> = ({
  filters,
  onFiltersChange,
  availableTags,
  className = ''
}) => {
  const [isOpen, setIsOpen] = useState(false);

  const updateFilters = (updates: Partial<FilterOptions>) => {
    onFiltersChange({ ...filters, ...updates });
  };

  const toggleStatus = (status: 'ToDo' | 'InProgress' | 'Done') => {
    const newStatus = filters.status.includes(status)
      ? filters.status.filter(s => s !== status)
      : [...filters.status, status];
    updateFilters({ status: newStatus });
  };

  const togglePriority = (priority: 'Low' | 'Medium' | 'High') => {
    const newPriority = filters.priority.includes(priority)
      ? filters.priority.filter(p => p !== priority)
      : [...filters.priority, priority];
    updateFilters({ priority: newPriority });
  };

  const toggleTag = (tag: Tag) => {
    const isSelected = filters.tags.some(t => t.id === tag.id);
    const newTags = isSelected
      ? filters.tags.filter(t => t.id !== tag.id)
      : [...filters.tags, tag];
    updateFilters({ tags: newTags });
  };

  const updateDueDateRange = (field: 'start' | 'end', value: string) => {
    updateFilters({
      dueDateRange: {
        ...filters.dueDateRange,
        [field]: value || undefined
      }
    });
  };

  const updateCreatedDateRange = (field: 'start' | 'end', value: string) => {
    updateFilters({
      createdDateRange: {
        ...filters.createdDateRange,
        [field]: value || undefined
      }
    });
  };

  const clearAllFilters = () => {
    onFiltersChange({
      status: [],
      priority: [],
      tags: [],
      dueDateRange: {},
      createdDateRange: {}
    });
  };

  const hasActiveFilters = () => {
    return filters.status.length > 0 ||
           filters.priority.length > 0 ||
           filters.tags.length > 0 ||
           filters.dueDateRange.start ||
           filters.dueDateRange.end ||
           filters.createdDateRange.start ||
           filters.createdDateRange.end;
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'ToDo': return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200';
      case 'InProgress': return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300';
      case 'Done': return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300';
      default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Low': return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300';
      case 'Medium': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300';
      case 'High': return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300';
      default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200';
    }
  };

  return (
    <div className={`relative ${className}`}>
      {/* Filter Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={`inline-flex items-center px-3 py-2 border shadow-sm text-sm leading-4 font-medium rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors ${
          hasActiveFilters()
            ? 'border-blue-300 text-blue-700 bg-blue-50 dark:bg-blue-900/20 dark:text-blue-300 dark:border-blue-600'
            : 'border-gray-300 dark:border-gray-500 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-600 hover:bg-gray-50 dark:hover:bg-gray-500'
        }`}
      >
        <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
        </svg>
        Filters
        {hasActiveFilters() && (
          <span className="ml-2 inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300">
            {filters.status.length + filters.priority.length + filters.tags.length}
          </span>
        )}
      </button>

      {/* Filter Dropdown */}
      {isOpen && (
        <div className="absolute right-0 mt-2 w-80 bg-white dark:bg-gray-700 rounded-lg shadow-lg border border-gray-200 dark:border-gray-600 z-50">
          <div className="p-4">
            {/* Header */}
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100">Filter Tasks</h3>
              {hasActiveFilters() && (
                <button
                  onClick={clearAllFilters}
                  className="text-sm text-gray-500 hover:text-gray-700 dark:hover:text-gray-300 underline"
                >
                  Clear all
                </button>
              )}
            </div>

            {/* Status Filter */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Status
              </label>
              <div className="flex flex-wrap gap-2">
                {['ToDo', 'InProgress', 'Done'].map(status => (
                  <button
                    key={status}
                    onClick={() => toggleStatus(status as 'ToDo' | 'InProgress' | 'Done')}
                    className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
                      filters.status.includes(status as 'ToDo' | 'InProgress' | 'Done')
                        ? getStatusColor(status)
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200 dark:bg-gray-600 dark:text-gray-400 dark:hover:bg-gray-500'
                    }`}
                  >
                    {status.replace(/([A-Z])/g, ' $1').trim()}
                  </button>
                ))}
              </div>
            </div>

            {/* Priority Filter */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Priority
              </label>
              <div className="flex flex-wrap gap-2">
                {['Low', 'Medium', 'High'].map(priority => (
                  <button
                    key={priority}
                    onClick={() => togglePriority(priority as 'Low' | 'Medium' | 'High')}
                    className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
                      filters.priority.includes(priority as 'Low' | 'Medium' | 'High')
                        ? getPriorityColor(priority)
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200 dark:bg-gray-600 dark:text-gray-400 dark:hover:bg-gray-500'
                    }`}
                  >
                    {priority}
                  </button>
                ))}
              </div>
            </div>

            {/* Tags Filter */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Tags
              </label>
              <div className="max-h-32 overflow-y-auto">
                <div className="flex flex-wrap gap-2">
                  {availableTags.map(tag => (
                    <button
                      key={tag.id}
                      onClick={() => toggleTag(tag)}
                      className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
                        filters.tags.some(t => t.id === tag.id)
                          ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300'
                          : 'bg-gray-100 text-gray-600 hover:bg-gray-200 dark:bg-gray-600 dark:text-gray-400 dark:hover:bg-gray-500'
                      }`}
                    >
                      {tag.name}
                    </button>
                  ))}
                </div>
              </div>
            </div>

            {/* Due Date Range */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Due Date Range
              </label>
              <div className="grid grid-cols-2 gap-2">
                <div>
                  <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">From</label>
                  <input
                    type="date"
                    value={filters.dueDateRange.start || ''}
                    onChange={(e) => updateDueDateRange('start', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 dark:border-gray-500 rounded-md text-sm bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">To</label>
                  <input
                    type="date"
                    value={filters.dueDateRange.end || ''}
                    onChange={(e) => updateDueDateRange('end', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 dark:border-gray-500 rounded-md text-sm bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
              </div>
            </div>

            {/* Created Date Range */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Created Date Range
              </label>
              <div className="grid grid-cols-2 gap-2">
                <div>
                  <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">From</label>
                  <input
                    type="date"
                    value={filters.createdDateRange.start || ''}
                    onChange={(e) => updateCreatedDateRange('start', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 dark:border-gray-500 rounded-md text-sm bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
                <div>
                  <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">To</label>
                  <input
                    type="date"
                    value={filters.createdDateRange.end || ''}
                    onChange={(e) => updateCreatedDateRange('end', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 dark:border-gray-500 rounded-md text-sm bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Click outside to close */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  );
};

export default TaskFilter;
