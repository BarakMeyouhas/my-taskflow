import React from 'react';
import Header from './Header';
import Sidebar from './Sidebar';
import SearchBar from './SearchBar';
import TaskBoard from './TaskBoard';

const Dashboard: React.FC = () => {
  // Sample task data
  const sampleTasks = [
    {
      id: '1',
      title: 'Design new landing page',
      description: 'Create a modern, responsive landing page design for the new product launch',
      status: 'todo' as const,
      priority: 'high' as const,
      assignee: 'Alex Johnson',
      dueDate: 'Dec 20, 2024',
      tags: ['Design', 'Frontend']
    },
    {
      id: '2',
      title: 'Implement user authentication',
      description: 'Set up secure user authentication with JWT tokens and refresh logic',
      status: 'in-progress' as const,
      priority: 'urgent' as const,
      assignee: 'Sarah Miller',
      dueDate: 'Dec 18, 2024',
      tags: ['Backend', 'Security']
    },
    {
      id: '3',
      title: 'Write API documentation',
      description: 'Create comprehensive API documentation with examples and error codes',
      status: 'review' as const,
      priority: 'medium' as const,
      assignee: 'Mike Kim',
      dueDate: 'Dec 22, 2024',
      tags: ['Documentation', 'API']
    },
    {
      id: '4',
      title: 'Fix mobile navigation bug',
      description: 'Resolve the navigation menu not working properly on mobile devices',
      status: 'done' as const,
      priority: 'high' as const,
      assignee: 'Sarah Miller',
      dueDate: 'Dec 15, 2024',
      tags: ['Bug Fix', 'Mobile']
    },
    {
      id: '5',
      title: 'Set up CI/CD pipeline',
      description: 'Configure automated testing and deployment pipeline for the project',
      status: 'todo' as const,
      priority: 'medium' as const,
      assignee: 'John Doe',
      dueDate: 'Dec 25, 2024',
      tags: ['DevOps', 'Automation']
    },
    {
      id: '6',
      title: 'Conduct user testing',
      description: 'Organize and conduct user testing sessions for the beta version',
      status: 'in-progress' as const,
      priority: 'low' as const,
      assignee: 'Alex Johnson',
      dueDate: 'Dec 28, 2024',
      tags: ['Testing', 'UX']
    },
    {
      id: '7',
      title: 'Update privacy policy',
      description: 'Review and update the privacy policy to comply with new regulations',
      status: 'todo' as const,
      priority: 'medium' as const,
      assignee: 'John Doe',
      dueDate: 'Dec 30, 2024',
      tags: ['Legal', 'Compliance']
    },
    {
      id: '8',
      title: 'Optimize database queries',
      description: 'Analyze and optimize slow database queries to improve performance',
      status: 'review' as const,
      priority: 'high' as const,
      assignee: 'Sarah Miller',
      dueDate: 'Dec 19, 2024',
      tags: ['Backend', 'Performance']
    }
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <div className="flex">
        <Sidebar />
        <main className="flex-1">
          <SearchBar />
          <div className="p-6">
            <div className="mb-6">
              <h1 className="text-2xl font-bold text-gray-900">Task Board</h1>
              <p className="text-gray-600">Manage your team&apos;s tasks and track progress</p>
            </div>
            <TaskBoard tasks={sampleTasks} />
          </div>
        </main>
      </div>
    </div>
  );
};

export default Dashboard;
