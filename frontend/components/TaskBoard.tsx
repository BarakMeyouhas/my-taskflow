import React, { useState } from 'react';
import TaskCard from './TaskCard';
import TaskCreateForm from './TaskCreateForm';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';

interface TaskBoardProps {
  tasks: Task[];
  onCreateTask?: (taskData: CreateTaskRequest) => Promise<void>;
  onUpdateTask?: (taskId: number, taskData: UpdateTaskRequest) => Promise<void>;
  onDeleteTask?: (taskId: number) => Promise<void>;
}

const TaskBoard: React.FC<TaskBoardProps> = ({ 
  tasks, 
  onCreateTask, 
  onUpdateTask, 
  onDeleteTask 
}) => {
  const [isCreateFormOpen, setIsCreateFormOpen] = useState(false);

  const columns = [
    { id: 'ToDo', title: 'To Do', color: 'bg-gray-100' },
    { id: 'InProgress', title: 'In Progress', color: 'bg-blue-100' },
    { id: 'Done', title: 'Done', color: 'bg-green-100' }
  ];

  const getTasksByStatus = (status: string) => {
    return tasks.filter(task => task.status === status);
  };

  const handleCreateTask = async (taskData: CreateTaskRequest) => {
    if (onCreateTask) {
      await onCreateTask(taskData);
    }
  };

  return (
    <>
      <div className="flex space-x-6 overflow-x-auto pb-4">
        {columns.map((column) => (
          <div key={column.id} className="flex-shrink-0 w-80">
            <div className={`${column.color} rounded-lg p-4 mb-4`}>
              <div className="flex items-center justify-between mb-4">
                <h3 className="font-semibold text-gray-800">{column.title}</h3>
                <span className="bg-white bg-opacity-50 text-gray-700 text-sm font-medium px-2 py-1 rounded-full">
                  {getTasksByStatus(column.id).length}
                </span>
              </div>
            </div>
            
            <div className="space-y-4">
              {getTasksByStatus(column.id).map((task) => (
                <TaskCard 
                  key={task.id} 
                  {...task} 
                  onUpdateTask={onUpdateTask}
                  onDeleteTask={onDeleteTask}
                />
              ))}
              
              {/* Add Task Button */}
              <button 
                onClick={() => setIsCreateFormOpen(true)}
                className="w-full p-4 border-2 border-dashed border-gray-300 rounded-lg text-gray-500 hover:border-gray-400 hover:text-gray-600 transition-colors flex items-center justify-center space-x-2"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                <span className="text-sm font-medium">Add Task</span>
              </button>
            </div>
          </div>
        ))}
      </div>

      <TaskCreateForm
        isOpen={isCreateFormOpen}
        onClose={() => setIsCreateFormOpen(false)}
        onSubmit={handleCreateTask}
      />
    </>
  );
};

export default TaskBoard;
