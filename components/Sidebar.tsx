import React from 'react';

const Sidebar: React.FC = () => {
  const projects = [
    { id: 1, name: 'Website Redesign', color: 'bg-blue-500' },
    { id: 2, name: 'Mobile App', color: 'bg-green-500' },
    { id: 3, name: 'Marketing Campaign', color: 'bg-purple-500' },
    { id: 4, name: 'Bug Fixes', color: 'bg-red-500' }
  ];

  const filters = [
    { id: 'all', name: 'All Tasks', count: 24 },
    { id: 'my-tasks', name: 'My Tasks', count: 8 },
    { id: 'overdue', name: 'Overdue', count: 3 },
    { id: 'due-today', name: 'Due Today', count: 5 },
    { id: 'due-week', name: 'Due This Week', count: 12 }
  ];

  return (
    <aside className="w-64 shrink-0 bg-white border-r border-gray-200 h-[calc(100vh-4rem)] overflow-y-auto">
      {/* User Profile Section */}
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-gradient-to-r from-blue-500 to-purple-600 rounded-full flex items-center justify-center">
            <span className="text-white font-semibold text-sm">JD</span>
          </div>
          <div>
            <h3 className="font-semibold text-gray-900">John Doe</h3>
            <p className="text-sm text-gray-500">Product Manager</p>
          </div>
        </div>
      </div>

      {/* Quick Stats */}
      <div className="p-6 border-b border-gray-200">
        <h4 className="text-sm font-medium text-gray-900 mb-3">Quick Stats</h4>
        <div className="grid grid-cols-2 gap-3">
          <div className="bg-blue-50 p-3 rounded-lg">
            <p className="text-2xl font-bold text-blue-600">12</p>
            <p className="text-xs text-blue-600">Active Tasks</p>
          </div>
          <div className="bg-green-50 p-3 rounded-lg">
            <p className="text-2xl font-bold text-green-600">8</p>
            <p className="text-xs text-green-600">Completed</p>
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="p-6 border-b border-gray-200">
        <h4 className="text-sm font-medium text-gray-900 mb-3">Filters</h4>
        <div className="space-y-2">
          {filters.map((filter) => (
            <button
              key={filter.id}
              className="w-full flex items-center justify-between p-2 text-left text-sm text-gray-600 hover:bg-gray-50 rounded-md transition-colors"
            >
              <span>{filter.name}</span>
              <span className="bg-gray-100 text-gray-600 text-xs px-2 py-1 rounded-full">
                {filter.count}
              </span>
            </button>
          ))}
        </div>
      </div>

      {/* Projects */}
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-center justify-between mb-3">
          <h4 className="text-sm font-medium text-gray-900">Projects</h4>
          <button className="text-blue-600 hover:text-blue-700">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
          </button>
        </div>
        <div className="space-y-2">
          {projects.map((project) => (
            <button
              key={project.id}
              className="w-full flex items-center space-x-3 p-2 text-left text-sm text-gray-600 hover:bg-gray-50 rounded-md transition-colors"
            >
              <div className={`w-3 h-3 ${project.color} rounded-full`}></div>
              <span className="truncate">{project.name}</span>
            </button>
          ))}
        </div>
      </div>

      {/* Team Members */}
      <div className="p-6">
        <h4 className="text-sm font-medium text-gray-900 mb-3">Team Members</h4>
        <div className="space-y-3">
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
              <span className="text-white text-xs font-medium">SM</span>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-900">Sarah Miller</p>
              <p className="text-xs text-gray-500">Developer</p>
            </div>
          </div>
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
              <span className="text-white text-xs font-medium">AJ</span>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-900">Alex Johnson</p>
              <p className="text-xs text-gray-500">Designer</p>
            </div>
          </div>
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 bg-purple-500 rounded-full flex items-center justify-center">
              <span className="text-white text-xs font-medium">MK</span>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-900">Mike Kim</p>
              <p className="text-xs text-gray-500">QA Engineer</p>
            </div>
          </div>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;
