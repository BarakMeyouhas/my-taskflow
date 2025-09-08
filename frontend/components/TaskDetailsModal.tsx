import React from 'react';
import Modal from './ui/Modal';
import ModalHeader from './ui/ModalHeader';
import ModalBody from './ui/ModalBody';
import ModalFooter from './ui/ModalFooter';
import { Button, IconButton, TagBadge } from './ui';
import { Task } from '../types/task';

export interface TaskDetailsModalProps {
  isOpen: boolean;
  onClose: () => void;
  task: Task | null;
  onEdit?: () => void;
  onDelete?: () => void;
}

const TaskDetailsModal: React.FC<TaskDetailsModalProps> = ({
  isOpen,
  onClose,
  task,
  onEdit,
  onDelete,
}) => {
  if (!task) return null;

  const getStatusColor = (status: string) => {
    switch (status) {
      case "ToDo":
        return "badge-neutral";
      case "InProgress":
        return "badge-info";
      case "Done":
        return "badge-success";
      default:
        return "badge-neutral";
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case "Low":
        return "badge-neutral";
      case "Medium":
        return "badge-info";
      case "High":
        return "badge-warning";
      default:
        return "badge-neutral";
    }
  };

  const getPriorityIcon = (priority: string) => {
    switch (priority) {
      case "High":
        return "⚡";
      case "Medium":
        return "📌";
      case "Low":
        return "📝";
      default:
        return "📝";
    }
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "No due date";
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const EditIcon = (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
    </svg>
  );

  const DeleteIcon = (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
    </svg>
  );

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalHeader title={task.title} onClose={onClose}>
        <div className="flex items-center gap-2 mt-2">
          <span className={`${getStatusColor(task.status)}`}>
            {task.status.replace(/([A-Z])/g, " $1").trim()}
          </span>
          <span className={`${getPriorityColor(task.priority)}`}>
            {getPriorityIcon(task.priority)} {task.priority}
          </span>
        </div>
      </ModalHeader>

      <ModalBody>
        <div className="space-y-6">
          {/* Description */}
          {task.description && (
            <div>
              <h3 className="text-label text-gray-700 space-small">Description</h3>
              <p className="text-body text-gray-600 whitespace-pre-wrap">{task.description}</p>
            </div>
          )}

          {/* Tags */}
          {task.tags && task.tags.length > 0 && (
            <div>
              <h3 className="text-label text-gray-700 space-small">Tags</h3>
              <div className="flex flex-wrap gap-2">
                {task.tags.map(tag => (
                  <TagBadge key={tag.id} tag={tag} size="md" />
                ))}
              </div>
            </div>
          )}

          {/* Task Details Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Due Date */}
            <div>
              <h3 className="text-label text-gray-700 space-small">Due Date</h3>
              <div className="flex items-center gap-2">
                <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <span className="text-body text-gray-600">{formatDate(task.dueDate)}</span>
              </div>
            </div>

            {/* Owner */}
            <div>
              <h3 className="text-label text-gray-700 space-small">Owner</h3>
              <div className="flex items-center gap-2">
                <div className="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-full flex items-center justify-center">
                  <span className="text-white text-sm font-medium">
                    {task.owner?.username?.charAt(0).toUpperCase() || '?'}
                  </span>
                </div>
                <span className="text-body text-gray-600">{task.owner?.username || 'Unknown'}</span>
              </div>
            </div>

            {/* Created Date */}
            <div>
              <h3 className="text-label text-gray-700 space-small">Created</h3>
              <span className="text-body text-gray-600">{formatDateTime(task.createdAt)}</span>
            </div>

            {/* Last Updated */}
            <div>
              <h3 className="text-label text-gray-700 space-small">Last Updated</h3>
              <span className="text-body text-gray-600">{formatDateTime(task.updatedAt)}</span>
            </div>
          </div>

          {/* Task ID */}
          <div className="pt-4 border-t border-gray-100">
            <span className="text-caption text-gray-400">Task ID: {task.id}</span>
          </div>
        </div>
      </ModalBody>

      <ModalFooter align="between">
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="sm"
            leftIcon={EditIcon}
            onClick={onEdit}
          >
            Edit Task
          </Button>
          <Button
            variant="ghost"
            size="sm"
            leftIcon={DeleteIcon}
            onClick={onDelete}
            className="text-error-600 hover:text-error-700 hover:bg-error-50"
          >
            Delete Task
          </Button>
        </div>
        <Button variant="outline" onClick={onClose}>
          Close
        </Button>
      </ModalFooter>
    </Modal>
  );
};

export default TaskDetailsModal;
