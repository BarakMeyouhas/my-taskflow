import React, { useState } from 'react';
import SearchBar from './SearchBar';
import TaskBoard from './TaskBoard';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';

const Dashboard: React.FC = () => {
  // Convert sample data to match new interface
  const [tasks, setTasks] = useState<Task[]>([
    {
      id: 1,
      title: 'Design new landing page',
      description: 'Create a modern, responsive landing page design for the new product launch',
      status: 'ToDo',
      priority: 'High',
      dueDate: '2024-12-20',
      createdAt: '2024-12-01T10:00:00Z',
      updatedAt: '2024-12-01T10:00:00Z',
      ownerId: 1,
      owner: {
        id: 1,
        username: 'Alex Johnson',
        email: 'alex@example.com'
      }
    },
    {
      id: 2,
      title: 'Implement user authentication',
      description: 'Set up secure user authentication with JWT tokens and refresh logic',
      status: 'InProgress',
      priority: 'High',
      dueDate: '2024-12-18',
      createdAt: '2024-12-01T10:00:00Z',
      updatedAt: '2024-12-01T10:00:00Z',
      ownerId: 2,
      owner: {
        id: 2,
        username: 'Sarah Miller',
        email: 'sarah@example.com'
      }
    },
    {
      id: 3,
      title: 'Write API documentation',
      description: 'Create comprehensive API documentation with examples and error codes',
      status: 'Done',
      priority: 'Medium',
      dueDate: '2024-12-22',
      createdAt: '2024-12-01T10:00:00Z',
      updatedAt: '2024-12-01T10:00:00Z',
      ownerId: 3,
      owner: {
        id: 3,
        username: 'Mike Kim',
        email: 'mike@example.com'
      }
    }
  ]);

  const handleCreateTask = async (taskData: CreateTaskRequest) => {
    // TODO: Replace with actual API call
    const newTask: Task = {
      id: Math.max(...tasks.map(t => t.id)) + 1, // Generate new ID
      title: taskData.title,
      description: taskData.description,
      status: 'ToDo', // New tasks start as ToDo
      priority: taskData.priority || 'Medium',
      dueDate: taskData.dueDate,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      ownerId: 1, // TODO: Get from auth context
      owner: {
        id: 1,
        username: 'Current User', // TODO: Get from auth context
        email: 'user@example.com'
      }
    };

    setTasks(prev => [...prev, newTask]);
    console.log('Task created:', newTask);
  };

  const handleUpdateTask = async (taskId: number, taskData: UpdateTaskRequest) => {
    // TODO: Replace with actual API call
    setTasks(prev => 
      prev.map(task => 
        task.id === taskId 
          ? { 
              ...task, 
              ...taskData, 
              updatedAt: new Date().toISOString() 
            }
          : task
      )
    );
    console.log('Task updated:', taskId, taskData);
  };

  return (
    <div className="min-w-0">
      <SearchBar />
      <div className="p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Task Board</h1>
          <p className="text-gray-600">Manage your team&apos;s tasks and track progress</p>
        </div>
        <TaskBoard 
          tasks={tasks} 
          onCreateTask={handleCreateTask}
          onUpdateTask={handleUpdateTask}
        />
      </div>
    </div>
  );
};

export default Dashboard;
