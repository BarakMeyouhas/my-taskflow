import React, { useState, useEffect } from 'react';
import SearchBar from './SearchBar';
import TaskBoard from './TaskBoard';
import TagManager from './TagManager';
import { Button } from './ui';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';
import { Tag } from '../types/tag';
import { FilterOptions } from './TaskFilter';
import { taskService } from '../services/taskService';
import { tagService } from '../services/tagService';

const Dashboard: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [filteredTasks, setFilteredTasks] = useState<Task[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isTagManagerOpen, setIsTagManagerOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [isSearching, setIsSearching] = useState(false);
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [filters, setFilters] = useState<FilterOptions>({
    status: [],
    priority: [],
    tags: [],
    dueDateRange: {},
    createdDateRange: {}
  });

  // Load tasks from API on component mount
  useEffect(() => {
    loadTasks();
    loadAvailableTags();
  }, []);

  // Filter tasks when selectedTags or searchTerm change
  useEffect(() => {
    if (searchTerm.trim()) {
      performSearch();
    } else {
      applyFilters();
    }
  }, [tasks, filters, searchTerm]);

  const performSearch = async () => {
    if (!searchTerm.trim()) {
      applyFilters();
      return;
    }

    try {
      setIsSearching(true);
      const searchResults = await taskService.searchTasks(searchTerm);
      
      // Apply comprehensive filters to search results
      const filtered = applyFiltersToTasks(searchResults);
      setFilteredTasks(filtered);
    } catch (error) {
      console.error('Search failed:', error);
      setError('Search failed. Please try again.');
      // Fallback to local filtering
      applyFilters();
    } finally {
      setIsSearching(false);
    }
  };

  const applyFilters = () => {
    const filtered = applyFiltersToTasks(tasks);
    setFilteredTasks(filtered);
  };

  const applyFiltersToTasks = (tasksToFilter: Task[]): Task[] => {
    return tasksToFilter.filter(task => {
      // Status filter
      if (filters.status.length > 0 && !filters.status.includes(task.status)) {
        return false;
      }

      // Priority filter
      if (filters.priority.length > 0 && !filters.priority.includes(task.priority)) {
        return false;
      }

      // Tags filter
      if (filters.tags.length > 0) {
        if (!task.tags || task.tags.length === 0) {
          return false;
        }
        
        const hasMatchingTag = filters.tags.some(filterTag => 
          task.tags!.some(taskTag => taskTag.id === filterTag.id)
        );
        
        if (!hasMatchingTag) {
          return false;
        }
      }

      // Due date range filter
      if (filters.dueDateRange.start || filters.dueDateRange.end) {
        if (!task.dueDate) {
          return false;
        }
        
        const taskDueDate = new Date(task.dueDate);
        
        if (filters.dueDateRange.start) {
          const startDate = new Date(filters.dueDateRange.start);
          if (taskDueDate < startDate) {
            return false;
          }
        }
        
        if (filters.dueDateRange.end) {
          const endDate = new Date(filters.dueDateRange.end);
          if (taskDueDate > endDate) {
            return false;
          }
        }
      }

      // Created date range filter
      if (filters.createdDateRange.start || filters.createdDateRange.end) {
        const taskCreatedDate = new Date(task.createdAt);
        
        if (filters.createdDateRange.start) {
          const startDate = new Date(filters.createdDateRange.start);
          if (taskCreatedDate < startDate) {
            return false;
          }
        }
        
        if (filters.createdDateRange.end) {
          const endDate = new Date(filters.createdDateRange.end);
          if (taskCreatedDate > endDate) {
            return false;
          }
        }
      }

      return true;
    });
  };

  const loadAvailableTags = async () => {
    try {
      const tags = await tagService.getTags();
      setAvailableTags(tags);
    } catch (error) {
      console.error('Failed to load tags:', error);
    }
  };

  const handleSearchChange = (newSearchTerm: string) => {
    setSearchTerm(newSearchTerm);
    setIsSearching(newSearchTerm.length > 0);
  };

  const hasActiveFilters = (): boolean => {
    return filters.status.length > 0 ||
           filters.priority.length > 0 ||
           filters.tags.length > 0 ||
           !!filters.dueDateRange.start ||
           !!filters.dueDateRange.end ||
           !!filters.createdDateRange.start ||
           !!filters.createdDateRange.end;
  };

  const getFilterSummary = (): string => {
    const parts: string[] = [];
    
    if (filters.status.length > 0) {
      parts.push(`${filters.status.length} status`);
    }
    if (filters.priority.length > 0) {
      parts.push(`${filters.priority.length} priority`);
    }
    if (filters.tags.length > 0) {
      parts.push(`${filters.tags.length} tag${filters.tags.length !== 1 ? 's' : ''}`);
    }
    if (filters.dueDateRange.start || filters.dueDateRange.end) {
      parts.push('due date range');
    }
    if (filters.createdDateRange.start || filters.createdDateRange.end) {
      parts.push('created date range');
    }
    
    return `Filtered by ${parts.join(', ')}`;
  };

  const loadTasks = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const fetchedTasks = await taskService.getTasks();
      setTasks(fetchedTasks);
    } catch (error) {
      console.error('Failed to load tasks:', error);
      setError(error instanceof Error ? error.message : 'Failed to load tasks');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateTask = async (taskData: CreateTaskRequest, selectedTags?: Tag[]) => {
    try {
      const newTask = await taskService.createTask(taskData);
      
      // Assign tags to the new task if any were selected
      if (selectedTags && selectedTags.length > 0) {
        for (const tag of selectedTags) {
          try {
            await tagService.assignTagToTask(newTask.id, tag.id);
          } catch (tagError) {
            console.error(`Failed to assign tag ${tag.name} to task:`, tagError);
          }
        }
        
        // Reload tasks to get updated tag information
        await loadTasks();
      } else {
        setTasks(prev => [...prev, newTask]);
      }
    } catch (error) {
      console.error('Failed to create task:', error);
      throw error; // Re-throw to let TaskCreateForm handle the error
    }
  };

  const handleUpdateTask = async (taskId: number, taskData: UpdateTaskRequest, selectedTags?: Tag[]) => {
    try {
      const updatedTask = await taskService.updateTask(taskId, taskData);
      
      // Handle tag assignments if tags were provided
      if (selectedTags) {
        // Get current task to compare tags
        const currentTask = tasks.find(t => t.id === taskId);
        const currentTagIds = currentTask?.tags?.map(tag => tag.id) || [];
        const selectedTagIds = selectedTags.map(tag => tag.id);
        
        // Find tags to add and remove
        const tagsToAdd = selectedTagIds.filter(id => !currentTagIds.includes(id));
        const tagsToRemove = currentTagIds.filter(id => !selectedTagIds.includes(id));
        
        // Remove tags that are no longer selected
        for (const tagId of tagsToRemove) {
          try {
            await tagService.removeTagFromTask(taskId, tagId);
          } catch (tagError) {
            console.error(`Failed to remove tag ${tagId} from task:`, tagError);
          }
        }
        
        // Add new tags
        for (const tagId of tagsToAdd) {
          try {
            await tagService.assignTagToTask(taskId, tagId);
          } catch (tagError) {
            console.error(`Failed to assign tag ${tagId} to task:`, tagError);
          }
        }
        
        // Reload tasks to get updated tag information
        await loadTasks();
      } else {
        // No tag changes, just update the task
        setTasks(prev => 
          prev.map(task => 
            task.id === taskId ? updatedTask : task
          )
        );
      }
    } catch (error) {
      console.error('Failed to update task:', error);
      throw error; // Re-throw to let TaskEditForm handle the error
    }
  };

  const handleDeleteTask = async (taskId: number) => {
    try {
      await taskService.deleteTask(taskId);
      setTasks(prev => prev.filter(task => task.id !== taskId));
    } catch (error) {
      console.error('Failed to delete task:', error);
      throw error;
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 dark:bg-gray-800 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600 dark:text-gray-400">Loading tasks...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 dark:bg-gray-800 flex items-center justify-center">
        <div className="text-center">
          <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-300 px-4 py-3 rounded-md">
            <p className="font-medium">Error loading tasks</p>
            <p className="text-sm mt-1">{error}</p>
            <button 
              onClick={loadTasks}
              className="mt-3 px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 transition-colors"
            >
              Retry
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-w-0 bg-gray-50 dark:bg-gray-800">
      <SearchBar 
        filters={filters}
        onFiltersChange={setFilters}
        onSearchChange={handleSearchChange}
        availableTags={availableTags}
      />
      <div className="p-6">
        <div className="mb-6">
          <div className="flex justify-between items-start">
            <div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Task Board</h1>
              <p className="text-gray-600 dark:text-gray-400">
                Manage your team&apos;s tasks and track progress
                {isSearching && (
                  <span className="ml-2 text-blue-600 dark:text-blue-400">
                    • Searching for "{searchTerm}"...
                  </span>
                )}
                {searchTerm && !isSearching && (
                  <span className="ml-2 text-green-600 dark:text-green-400">
                    • Found {filteredTasks.length} result{filteredTasks.length !== 1 ? 's' : ''} for "{searchTerm}"
                  </span>
                )}
                {!searchTerm && hasActiveFilters() && (
                  <span className="ml-2 text-blue-600 dark:text-blue-400">
                    • {getFilterSummary()}
                  </span>
                )}
              </p>
            </div>
            <Button
              variant="outline"
              onClick={() => setIsTagManagerOpen(true)}
              leftIcon={
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                </svg>
              }
            >
              Manage Tags
            </Button>
          </div>
        </div>
        <TaskBoard 
          tasks={filteredTasks} 
          searchTerm={searchTerm}
          onCreateTask={handleCreateTask}
          onUpdateTask={handleUpdateTask}
          onDeleteTask={handleDeleteTask}
        />
      </div>

      {/* Tag Manager Modal */}
      <TagManager
        isOpen={isTagManagerOpen}
        onClose={() => setIsTagManagerOpen(false)}
      />
    </div>
  );
};

export default Dashboard;
