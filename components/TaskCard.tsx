import React from 'react';

interface TaskCardProps {
  id: string;
  title: string;
  description: string;
  status: 'todo' | 'in-progress' | 'review' | 'done';
  priority: 'low' | 'medium' | 'high' | 'urgent';
  assignee: string;
  dueDate: string;
  tags: string[];
}

const TaskCard: React.FC<TaskCardProps> = ({
  id,
  title,
  description,
  status,
  priority,
  assignee,
  dueDate,
  tags
}) => {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'todo': return 'bg-gray-100 text-gray-800';
      case 'in-progress': return 'bg-blue-100 text-blue-800';
      case 'review': return 'bg-yellow-100 text-yellow-800';
      case 'done': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'low': return 'bg-gray-100 text-gray-600';
      case 'medium': return 'bg-blue-100 text-blue-600';
      case 'high': return 'bg-orange-100 text-orange-600';
      case 'urgent': return 'bg-red-100 text-red-600';
      default: return 'bg-gray-100 text-gray-600';
    }
  };

  const getPriorityIcon = (priority: string) => {
    switch (priority) {
      case 'urgent': return '🔥';
      case 'high': return '⚡';
      case 'medium': return '📌';
      case 'low': return '📝';
      default: return '📝';
    }
  };

  return (
    <div className="bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow duration-200 p-6">
      {/* Header */}
      <div className="flex items-start justify-between mb-4">
        <h3 className="text-lg font-semibold text-gray-900 truncate">{title}</h3>
        <div className="flex items-center space-x-2">
          <span className={`px-2 py-1 rounded-full text-xs font-medium ${getPriorityColor(priority)}`}>
            {getPriorityIcon(priority)} {priority}
          </span>
        </div>
      </div>

      {/* Description */}
      <p className="text-gray-600 text-sm mb-4 overflow-hidden text-ellipsis whitespace-nowrap">{description}</p>

      {/* Status and Assignee */}
      <div className="flex items-center justify-between mb-4">
        <span className={`px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(status)}`}>
          {status.replace('-', ' ')}
        </span>
        <div className="flex items-center space-x-2">
          <div className="w-6 h-6 bg-gray-300 rounded-full flex items-center justify-center">
            <svg className="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </div>
          <span className="text-sm text-gray-600">{assignee}</span>
        </div>
      </div>

      {/* Tags */}
      {tags.length > 0 && (
        <div className="flex flex-wrap gap-2 mb-4">
          {tags.map((tag, index) => (
            <span
              key={index}
              className="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded-md"
            >
              {tag}
            </span>
          ))}
        </div>
      )}

      {/* Due Date and Actions */}
      <div className="flex items-center justify-between pt-4 border-t border-gray-100">
        <div className="flex items-center space-x-2 text-sm text-gray-500">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          <span>Due {dueDate}</span>
        </div>
        
        <div className="flex items-center space-x-2">
          <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors rounded-md hover:bg-gray-100">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
          </button>
          <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors rounded-md hover:bg-gray-100">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
};

export default TaskCard;
