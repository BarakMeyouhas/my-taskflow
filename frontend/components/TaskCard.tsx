import React, { useState } from "react";
import TaskEditForm from "./TaskEditForm";
import TaskDetailsModal from "./TaskDetailsModal";
import { Button, IconButton } from "./ui";
import { Task, UpdateTaskRequest } from "../types/task";

interface TaskCardProps {
  id: number;
  title: string;
  description?: string;
  status: "ToDo" | "InProgress" | "Done";
  priority: "Low" | "Medium" | "High";
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
  ownerId: number;
  owner?: {
    id: number;
    username: string;
    email: string;
  };
  onUpdateTask?: (taskId: number, taskData: UpdateTaskRequest) => Promise<void>;
  onDeleteTask?: (taskId: number) => Promise<void>;
}

const TaskCard: React.FC<TaskCardProps> = ({
  id,
  title,
  description,
  status,
  priority,
  dueDate,
  createdAt,
  updatedAt,
  ownerId,
  owner,
  onUpdateTask,
  onDeleteTask,
}) => {
  const [isEditFormOpen, setIsEditFormOpen] = useState(false);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

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
    return new Date(dateString).toLocaleDateString();
  };

  const handleUpdateTask = async (taskId: number, taskData: UpdateTaskRequest) => {
    if (onUpdateTask) {
      await onUpdateTask(taskId, taskData);
    }
  };

  const handleDeleteTask = async () => {
    if (onDeleteTask) {
      await onDeleteTask(id);
      setShowDeleteConfirm(false);
    }
  };

  const currentTask: Task = {
    id,
    title,
    description,
    status,
    priority,
    dueDate,
    createdAt,
    updatedAt,
    ownerId,
    owner,
  };

  const EditIcon = (
    <svg
      className="w-4 h-4"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={2}
        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
      />
    </svg>
  );

  const DeleteIcon = (
    <svg
      className="w-4 h-4"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={2}
        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
      />
    </svg>
  );

  return (
    <>
      <div 
        className="card-interactive padding-component cursor-pointer"
        onClick={() => setIsDetailsModalOpen(true)}
      >
        {/* Header */}
        <div className="flex items-start justify-between space-element">
          <h3 className="text-heading-4 text-gray-900 dark:text-gray-100 truncate">
            {title}
          </h3>
          <div className="flex items-center gap-2">
            <span className={`${getPriorityColor(priority)}`}>
              {getPriorityIcon(priority)} {priority}
            </span>
          </div>
        </div>

        {/* Description */}
        {description && (
          <p className="text-body-small text-gray-600 dark:text-gray-300 space-element overflow-hidden text-ellipsis whitespace-nowrap">
            {description}
          </p>
        )}

        {/* Status and Owner */}
        <div className="flex items-center justify-between space-element">
          <span className={`${getStatusColor(status)}`}>
            {status.replace(/([A-Z])/g, " $1").trim()}
          </span>
          <div className="flex items-center gap-2">
            <div className="w-6 h-6 bg-gray-300 rounded-full flex items-center justify-center">
              <svg
                className="w-4 h-4 text-gray-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                />
              </svg>
            </div>
            <span className="text-caption text-gray-500 dark:text-gray-400">
              {owner?.username || "Unknown"}
            </span>
          </div>
        </div>

        {/* Due Date */}
        <div className="space-element">
          <div className="flex items-center gap-2">
            <svg
              className="w-4 h-4 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
              />
            </svg>
            <span className="text-caption text-gray-500 dark:text-gray-400">
              {formatDate(dueDate)}
            </span>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center justify-between pt-4 border-t border-gray-100 dark:border-gray-600">
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="sm"
              leftIcon={EditIcon}
              onClick={(e) => {
                e.stopPropagation();
                setIsEditFormOpen(true);
              }}
            >
              Edit
            </Button>
            <Button
              variant="ghost"
              size="sm"
              leftIcon={DeleteIcon}
              onClick={(e) => {
                e.stopPropagation();
                setShowDeleteConfirm(true);
              }}
              className="text-error-600 hover:text-error-700 hover:bg-error-50"
            >
              Delete
            </Button>
          </div>
          
          <div className="text-caption text-gray-400 dark:text-gray-500">
            Created {new Date(createdAt).toLocaleDateString()}
          </div>
        </div>
      </div>

      {/* Task Details Modal */}
      <TaskDetailsModal
        isOpen={isDetailsModalOpen}
        onClose={() => setIsDetailsModalOpen(false)}
        task={currentTask}
        onEdit={() => {
          setIsDetailsModalOpen(false);
          setIsEditFormOpen(true);
        }}
        onDelete={() => {
          setIsDetailsModalOpen(false);
          setShowDeleteConfirm(true);
        }}
      />

      {/* Edit Form Modal */}
      <TaskEditForm
        isOpen={isEditFormOpen}
        onClose={() => setIsEditFormOpen(false)}
        task={currentTask}
        onSubmit={handleUpdateTask}
      />

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white dark:bg-gray-700 rounded-lg padding-component max-w-md w-full mx-4">
            <h3 className="text-heading-4 text-gray-900 dark:text-gray-100 space-element">
              Delete Task
            </h3>
            <p className="text-body text-gray-600 dark:text-gray-300 space-element">
              Are you sure you want to delete "{title}"? This action cannot be undone.
            </p>
            <div className="flex items-center justify-end gap-3">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setShowDeleteConfirm(false)}
              >
                Cancel
              </Button>
              <Button
                variant="destructive"
                size="sm"
                onClick={handleDeleteTask}
              >
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default TaskCard;
