export interface Competence {
    id: string;
    name: string;
    description?: string;
}

export interface CreateCompetencyDto {
    name: string;
    description?: string;
}

export interface UpdateCompetencyDto {
    id: number;
    name: string;
    description?: string;
}

export interface DeleteCompetenceDto {
    id: number;
}

export interface ApiResponse {
    hasErrors: boolean;
    errors?: [];
}

export interface PaginatedResponse<T> {
    items: T[];
    total: number;
    page: number;
    pageSize: number;
    totalPages: number;
}