import { Tag } from './tag';

export interface Task {
  id: number;
  title: string;
  description?: string;
  status: 'ToDo' | 'InProgress' | 'Done';
  priority: 'Low' | 'Medium' | 'High';
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
  ownerId: number;
  owner?: {
    id: number;
    username: string;
    email: string;
  };
  tags?: Tag[];
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority?: 'Low' | 'Medium' | 'High';
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  status?: 'ToDo' | 'InProgress' | 'Done';
  priority?: 'Low' | 'Medium' | 'High';
  dueDate?: string;
}
