export interface Tag {
  id: number;
  name: string;
  color?: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
  createdByUserId: number;
  createdBy?: {
    id: number;
    username: string;
    email: string;
  };
}

export interface CreateTagRequest {
  name: string;
  color?: string;
  description?: string;
}

export interface UpdateTagRequest {
  name: string;
  color?: string;
  description?: string;
}

export interface TagSearchResponse {
  count: number;
  searchTerm?: string;
  tags: Tag[];
}

export interface TagListResponse {
  count: number;
  tags: Tag[];
}
