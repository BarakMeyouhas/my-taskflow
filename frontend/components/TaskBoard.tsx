import React, { useState, useCallback } from 'react';
import { DragDropContext, Droppable, Draggable, DropResult } from '@hello-pangea/dnd';
import TaskCard from './TaskCard';
import TaskCreateForm from './TaskCreateForm';
import { Button } from './ui';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';

interface TaskBoardProps {
  tasks: Task[];
  onCreateTask?: (taskData: CreateTaskRequest) => Promise<void>;
  onUpdateTask?: (taskId: number, taskData: UpdateTaskRequest) => Promise<void>;
  onDeleteTask?: (taskId: number) => Promise<void>;
  searchTerm?: string;
}

const TaskBoard: React.FC<TaskBoardProps> = ({ 
  tasks, 
  onCreateTask, 
  onUpdateTask, 
  onDeleteTask,
  searchTerm = ''
}) => {
  const [isCreateFormOpen, setIsCreateFormOpen] = useState(false);
  const [isUpdating, setIsUpdating] = useState(false);

  const columns = [
    { id: 'ToDo', title: 'To Do', color: 'bg-gray-100 dark:bg-gray-600', textColor: 'text-gray-800 dark:text-gray-200' },
    { id: 'InProgress', title: 'In Progress', color: 'bg-blue-100 dark:bg-blue-900/30', textColor: 'text-blue-800 dark:text-blue-200' },
    { id: 'Done', title: 'Done', color: 'bg-green-100 dark:bg-green-900/30', textColor: 'text-green-800 dark:text-green-200' }
  ];

  const getTasksByStatus = (status: string) => {
    return tasks.filter(task => task.status === status);
  };

  const handleCreateTask = async (taskData: CreateTaskRequest) => {
    if (onCreateTask) {
      await onCreateTask(taskData);
    }
  };

  const handleDragEnd = useCallback(async (result: DropResult) => {
    const { destination, source, draggableId } = result;

    // If dropped outside a droppable area
    if (!destination) {
      return;
    }

    // If dropped in the same position
    if (
      destination.droppableId === source.droppableId &&
      destination.index === source.index
    ) {
      return;
    }

    // Find the task being moved
    const task = tasks.find(t => t.id.toString() === draggableId);
    if (!task) return;

    // Update task status if moved to different column
    if (destination.droppableId !== source.droppableId) {
      const newStatus = destination.droppableId as 'ToDo' | 'InProgress' | 'Done';
      
      // Set updating state to prevent visual glitches
      setIsUpdating(true);
      
      try {
        if (onUpdateTask) {
          await onUpdateTask(task.id, { ...task, status: newStatus });
        }
      } finally {
        // Reset updating state after a short delay to allow for smooth transition
        setTimeout(() => setIsUpdating(false), 100);
      }
    }
  }, [tasks, onUpdateTask]);

  const AddIcon = (
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
    </svg>
  );

  return (
    <>
      <DragDropContext onDragEnd={handleDragEnd}>
        <div className="flex space-x-6 overflow-x-auto pb-4">
          {columns.map((column) => (
            <div key={column.id} className="flex-shrink-0 w-80">
              <div className={`${column.color} rounded-lg p-4 mb-4`}>
                <div className="flex items-center justify-between mb-4">
                  <h3 className={`font-semibold ${column.textColor}`}>{column.title}</h3>
                  <span className="bg-white dark:bg-gray-500 bg-opacity-50 dark:bg-opacity-50 text-gray-700 dark:text-gray-200 text-sm font-medium px-2 py-1 rounded-full">
                    {getTasksByStatus(column.id).length}
                  </span>
                </div>
              </div>
              
              <Droppable droppableId={column.id}>
                {(provided, snapshot) => (
                  <div
                    ref={provided.innerRef}
                    {...provided.droppableProps}
                    className={`space-y-4 min-h-[200px] ${
                      isUpdating ? '' : 'transition-all duration-200'
                    } ${
                      snapshot.isDraggingOver ? 'bg-blue-50 dark:bg-blue-900/20 rounded-lg p-2' : ''
                    }`}
                  >
                    {getTasksByStatus(column.id).map((task, index) => (
                      <Draggable
                        key={task.id}
                        draggableId={task.id.toString()}
                        index={index}
                      >
                        {(provided, snapshot) => (
                          <div
                            ref={provided.innerRef}
                            {...provided.draggableProps}
                            {...provided.dragHandleProps}
                            className={`${
                              isUpdating ? '' : 'transition-all duration-200'
                            } ${
                              snapshot.isDragging 
                                ? 'opacity-90 shadow-xl scale-105 z-50' 
                                : 'opacity-100'
                            }`}
                            style={{
                              ...provided.draggableProps.style,
                              // Disable transform during updates to prevent glitches
                              transform: snapshot.isDragging 
                                ? provided.draggableProps.style?.transform 
                                : isUpdating 
                                  ? 'none' 
                                  : 'none'
                            }}
                          >
                            <TaskCard 
                              {...task} 
                              searchTerm={searchTerm}
                              onUpdateTask={onUpdateTask}
                              onDeleteTask={onDeleteTask}
                            />
                          </div>
                        )}
                      </Draggable>
                    ))}
                    {provided.placeholder}
                    
                    {/* Add Task Button */}
                    <Button
                      variant="outline"
                      fullWidth
                      leftIcon={AddIcon}
                      onClick={() => setIsCreateFormOpen(true)}
                      className="border-dashed border-gray-300 dark:border-gray-500 text-gray-500 dark:text-gray-400 hover:border-gray-400 dark:hover:border-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
                    >
                      Add Task
                    </Button>
                  </div>
                )}
              </Droppable>
            </div>
          ))}
        </div>
      </DragDropContext>

      <TaskCreateForm
        isOpen={isCreateFormOpen}
        onClose={() => setIsCreateFormOpen(false)}
        onSubmit={handleCreateTask}
      />
    </>
  );
};

export default TaskBoard;
