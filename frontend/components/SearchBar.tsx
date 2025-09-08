import React, { useState, useEffect, useCallback } from 'react';
import TaskFilter, { FilterOptions } from './TaskFilter';
import { Tag } from '../types/tag';

interface SearchBarProps {
  filters?: FilterOptions;
  onFiltersChange?: (filters: FilterOptions) => void;
  onSearchChange?: (searchTerm: string) => void;
  availableTags?: Tag[];
}

const SearchBar: React.FC<SearchBarProps> = ({ 
  filters = {
    status: [],
    priority: [],
    tags: [],
    dueDateRange: {},
    createdDateRange: {}
  },
  onFiltersChange = () => {},
  onSearchChange = () => {},
  availableTags = []
}) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isSearching, setIsSearching] = useState(false);

  // Debounce search input
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      onSearchChange(searchTerm);
    }, 300); // 300ms debounce

    return () => clearTimeout(timeoutId);
  }, [searchTerm, onSearchChange]);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSearchTerm(value);
    setIsSearching(value.length > 0);
  };

  const clearSearch = () => {
    setSearchTerm('');
    setIsSearching(false);
    onSearchChange('');
  };
  return (
    <div className="bg-white dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600 px-6 py-4">
      <div className="flex items-center justify-between">
        {/* Search Input */}
        <div className="flex-1 max-w-lg">
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-gray-400 dark:text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              value={searchTerm}
              onChange={handleSearchChange}
              placeholder="Search tasks, projects, or team members..."
              className="block w-full pl-10 pr-10 py-2 border border-gray-300 dark:border-gray-500 rounded-md leading-5 bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:placeholder-gray-400 dark:focus:placeholder-gray-500 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            />
            {/* Clear search button */}
            {searchTerm && (
              <button
                onClick={clearSearch}
                className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
              >
                <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            )}
          </div>
        </div>

        {/* Quick Actions */}
        <div className="flex items-center space-x-3 ml-4">
          {/* Task Filter */}
          <TaskFilter
            filters={filters}
            onFiltersChange={onFiltersChange}
            availableTags={availableTags}
          />

          {/* Sort Button */}
          <button className="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-500 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-600 hover:bg-gray-50 dark:hover:bg-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12" />
            </svg>
            Sort
          </button>

          {/* Add Task Button */}
          <button className="inline-flex items-center px-4 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
            Add Task
          </button>
        </div>
      </div>

      {/* Quick Filters */}
      <div className="mt-3 flex items-center space-x-4">
        {isSearching ? (
          <div className="flex items-center space-x-2">
            <span className="text-sm text-blue-600 dark:text-blue-400 font-medium">
              Searching for: "{searchTerm}"
            </span>
            <button
              onClick={clearSearch}
              className="text-xs text-gray-500 hover:text-gray-700 dark:hover:text-gray-300 underline"
            >
              Clear search
            </button>
          </div>
        ) : (
          <>
            <span className="text-sm text-gray-500 dark:text-gray-400">Quick filters:</span>
            <div className="flex space-x-2">
          <button className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300 hover:bg-blue-200 dark:hover:bg-blue-900/50 transition-colors">
            Due Today
          </button>
          <button className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300 hover:bg-red-200 dark:hover:bg-red-900/50 transition-colors">
            Overdue
          </button>
          <button className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-900/50 transition-colors">
            My Tasks
          </button>
          <button className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-purple-100 dark:bg-purple-900/30 text-purple-800 dark:text-purple-300 hover:bg-purple-200 dark:hover:bg-purple-900/50 transition-colors">
            High Priority
          </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default SearchBar;
