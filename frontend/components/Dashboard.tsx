import React, { useState, useEffect } from 'react';
import SearchBar from './SearchBar';
import TaskBoard from './TaskBoard';
import TagManager from './TagManager';
import { Button } from './ui';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';
import { Tag } from '../types/tag';
import { taskService } from '../services/taskService';
import { tagService } from '../services/tagService';

const Dashboard: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isTagManagerOpen, setIsTagManagerOpen] = useState(false);

  // Load tasks from API on component mount
  useEffect(() => {
    loadTasks();
  }, []);

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

  const handleUpdateTask = async (taskId: number, taskData: UpdateTaskRequest) => {
    try {
      const updatedTask = await taskService.updateTask(taskId, taskData);
      setTasks(prev => 
        prev.map(task => 
          task.id === taskId ? updatedTask : task
        )
      );
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
      <SearchBar />
      <div className="p-6">
        <div className="mb-6">
          <div className="flex justify-between items-start">
            <div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Task Board</h1>
              <p className="text-gray-600 dark:text-gray-400">Manage your team&apos;s tasks and track progress</p>
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
          tasks={tasks} 
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
