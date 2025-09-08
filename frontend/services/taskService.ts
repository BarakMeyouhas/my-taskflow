import { Task, CreateTaskRequest, UpdateTaskRequest } from "../types/task";
import { authConfig } from "../config/auth";

class TaskService {
  private getAuthHeaders() {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No authentication token found");
    }
    return {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    };
  }

  private getBaseUrl() {
    return authConfig.backendUrl;
  }

  private convertStatusToString(status: number): 'ToDo' | 'InProgress' | 'Done' {
    switch (status) {
      case 0: return 'ToDo';
      case 1: return 'InProgress';
      case 2: return 'Done';
      default: return 'ToDo';
    }
  }

  private convertPriorityToString(priority: number): 'Low' | 'Medium' | 'High' {
    switch (priority) {
      case 0: return 'Low';
      case 1: return 'Medium';
      case 2: return 'High';
      default: return 'Medium';
    }
  }

  private convertStatusToNumber(status: 'ToDo' | 'InProgress' | 'Done'): number {
    switch (status) {
      case 'ToDo': return 0;
      case 'InProgress': return 1;
      case 'Done': return 2;
      default: return 0;
    }
  }

  private convertPriorityToNumber(priority: 'Low' | 'Medium' | 'High'): number {
    switch (priority) {
      case 'Low': return 0;
      case 'Medium': return 1;
      case 'High': return 2;
      default: return 1;
    }
  }

  async getTasks(): Promise<Task[]> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/task`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch tasks: ${response.statusText}`);
      }

      const data = await response.json();
      console.log('🔍 DEBUG: Raw API response:', data);
      
      const tasks = data.tasks || [];
      console.log('🔍 DEBUG: Tasks array:', tasks);
      
      if (tasks.length > 0) {
        console.log('🔍 DEBUG: First task:', tasks[0]);
        console.log('🔍 DEBUG: First task tags:', tasks[0].tags);
      }
      
      // Convert numeric status from backend to string format expected by frontend
      const convertedTasks = tasks.map((task: any) => ({
        ...task,
        status: this.convertStatusToString(task.status),
        priority: this.convertPriorityToString(task.priority)
      }));
      
      console.log('🔍 DEBUG: Converted tasks:', convertedTasks);
      return convertedTasks;
    } catch (error) {
      console.error("Error fetching tasks:", error);
      throw error;
    }
  }

  async getTaskById(id: number): Promise<Task> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/task/${id}`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch task: ${response.statusText}`);
      }

      const data = await response.json();
      
      // Convert numeric status and priority from backend to string format
      return {
        ...data,
        status: this.convertStatusToString(data.status),
        priority: this.convertPriorityToString(data.priority)
      };
    } catch (error) {
      console.error("Error fetching task:", error);
      throw error;
    }
  }

  async createTask(taskData: CreateTaskRequest): Promise<Task> {
    try {
      // Convert frontend data to backend format
      const backendData = {
        title: taskData.title,
        description: taskData.description,
        priority:
          taskData.priority === "Low"
            ? 0
            : taskData.priority === "Medium"
            ? 1
            : taskData.priority === "High"
            ? 2
            : 1, // Default to Medium
        dueDate: taskData.dueDate
          ? new Date(taskData.dueDate).toISOString()
          : null,
      };

      console.log("Creating task with data:", backendData);
      console.log("API URL:", `${this.getBaseUrl()}/api/task`);

      const response = await fetch(`${this.getBaseUrl()}/api/task`, {
        method: "POST",
        headers: this.getAuthHeaders(),
        body: JSON.stringify(backendData),
      });

      console.log("Response status:", response.status);

      if (!response.ok) {
        const errorText = await response.text();
        console.error("Error response:", errorText);

        let errorMessage = `Failed to create task: ${response.statusText}`;
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.message || errorData.error || errorMessage;
        } catch {
          errorMessage = errorText || errorMessage;
        }

        throw new Error(errorMessage);
      }

      const data = await response.json();
      console.log("Success response:", data);
      
      // Convert numeric status and priority from backend to string format
      return {
        ...data,
        status: this.convertStatusToString(data.status),
        priority: this.convertPriorityToString(data.priority)
      };
    } catch (error) {
      console.error("Error creating task:", error);
      throw error;
    }
  }

  async updateTask(id: number, taskData: UpdateTaskRequest): Promise<Task> {
    try {
      // Convert frontend data to backend format
      const backendData: any = {
        title: taskData.title,
        description: taskData.description,
      };

      // Only add dueDate if it has a valid value
      if (taskData.dueDate && taskData.dueDate.trim() !== '') {
        backendData.dueDate = taskData.dueDate;
      }

      // Only add status and priority if they have values
      if (taskData.status) {
        backendData.status = this.convertStatusToNumber(taskData.status);
      }
      if (taskData.priority) {
        backendData.priority = this.convertPriorityToNumber(taskData.priority);
      }

      console.log('🔍 DEBUG: Updating task with ID:', id);
      console.log('🔍 DEBUG: Backend data being sent:', backendData);
      console.log('🔍 DEBUG: Request URL:', `${this.getBaseUrl()}/api/task/${id}`);

      const response = await fetch(`${this.getBaseUrl()}/api/task/${id}`, {
        method: "PUT",
        headers: this.getAuthHeaders(),
        body: JSON.stringify(backendData),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        console.log('🔍 DEBUG: Backend error response:', errorData);
        console.log('🔍 DEBUG: Validation errors:', errorData.errors);
        console.log('🔍 DEBUG: Response status:', response.status);
        console.log('🔍 DEBUG: Response statusText:', response.statusText);
        throw new Error(
          errorData.message || `Failed to update task: ${response.statusText}`
        );
      }

      const data = await response.json();
      
      // Convert numeric status and priority from backend to string format
      return {
        ...data,
        status: this.convertStatusToString(data.status),
        priority: this.convertPriorityToString(data.priority)
      };
    } catch (error) {
      console.error("Error updating task:", error);
      throw error;
    }
  }

  async deleteTask(id: number): Promise<void> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/task/${id}`, {
        method: "DELETE",
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.message || `Failed to delete task: ${response.statusText}`
        );
      }
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  }

  async searchTasks(searchTerm: string): Promise<Task[]> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/task/search?q=${encodeURIComponent(searchTerm)}`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to search tasks: ${response.statusText}`);
      }

      const data = await response.json();
      const tasks = data.tasks || [];
      
      // Convert numeric status and priority from backend to string format
      return tasks.map((task: any) => ({
        ...task,
        status: this.convertStatusToString(task.status),
        priority: this.convertPriorityToString(task.priority)
      }));
    } catch (error) {
      console.error("Error searching tasks:", error);
      throw error;
    }
  }
}

export const taskService = new TaskService();
