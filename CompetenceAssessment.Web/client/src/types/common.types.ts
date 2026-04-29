export interface ApiResponse {
    hasErrors: boolean;
    errorValues?: Record<string, string>;
}


export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    currentPage: number;
    pageSize: number;
    totalPages: number;
}