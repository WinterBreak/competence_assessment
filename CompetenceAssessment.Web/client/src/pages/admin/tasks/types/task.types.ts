export interface Task {
    id: string;
    text: string;
    type: string;
    answer: string | null;
}

export interface CreateTaskDto {
    name: string;
    description: string;
}

export interface UpdateTaskDto {
    id: number;
    name?: string;
    description?: string;
}