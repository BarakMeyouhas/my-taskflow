import { Tag, CreateTagRequest, UpdateTagRequest, TagSearchResponse, TagListResponse } from "../types/tag";
import { authConfig } from "../config/auth";

class TagService {
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

  async getTags(): Promise<Tag[]> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch tags: ${response.statusText}`);
      }

      const data: TagListResponse = await response.json();
      return data.tags || [];
    } catch (error) {
      console.error("Error fetching tags:", error);
      throw error;
    }
  }

  async getTagById(id: number): Promise<Tag> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/${id}`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch tag: ${response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error("Error fetching tag:", error);
      throw error;
    }
  }

  async createTag(tagData: CreateTagRequest): Promise<Tag> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag`, {
        method: "POST",
        headers: this.getAuthHeaders(),
        body: JSON.stringify(tagData),
      });

      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = `Failed to create tag: ${response.statusText}`;
        
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.error || errorData.message || errorMessage;
        } catch {
          errorMessage = errorText || errorMessage;
        }

        throw new Error(errorMessage);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error("Error creating tag:", error);
      throw error;
    }
  }

  async updateTag(id: number, tagData: UpdateTagRequest): Promise<Tag> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/${id}`, {
        method: "PUT",
        headers: this.getAuthHeaders(),
        body: JSON.stringify(tagData),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.error || errorData.message || `Failed to update tag: ${response.statusText}`
        );
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error("Error updating tag:", error);
      throw error;
    }
  }

  async deleteTag(id: number): Promise<void> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/${id}`, {
        method: "DELETE",
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.error || errorData.message || `Failed to delete tag: ${response.statusText}`
        );
      }
    } catch (error) {
      console.error("Error deleting tag:", error);
      throw error;
    }
  }

  async searchTags(searchTerm: string): Promise<Tag[]> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/search?q=${encodeURIComponent(searchTerm)}`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to search tags: ${response.statusText}`);
      }

      const data: TagSearchResponse = await response.json();
      return data.tags || [];
    } catch (error) {
      console.error("Error searching tags:", error);
      throw error;
    }
  }

  async assignTagToTask(taskId: number, tagId: number): Promise<void> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/${tagId}/assign/${taskId}`, {
        method: "POST",
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.error || errorData.message || `Failed to assign tag to task: ${response.statusText}`
        );
      }
    } catch (error) {
      console.error("Error assigning tag to task:", error);
      throw error;
    }
  }

  async removeTagFromTask(taskId: number, tagId: number): Promise<void> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/${tagId}/assign/${taskId}`, {
        method: "DELETE",
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.error || errorData.message || `Failed to remove tag from task: ${response.statusText}`
        );
      }
    } catch (error) {
      console.error("Error removing tag from task:", error);
      throw error;
    }
  }

  async getTagsForTask(taskId: number): Promise<Tag[]> {
    try {
      const response = await fetch(`${this.getBaseUrl()}/api/tag/task/${taskId}`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch tags for task: ${response.statusText}`);
      }

      const data = await response.json();
      return data.tags || [];
    } catch (error) {
      console.error("Error fetching tags for task:", error);
      throw error;
    }
  }
}

export const tagService = new TagService();
