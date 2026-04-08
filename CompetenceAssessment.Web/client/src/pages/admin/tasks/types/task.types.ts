export interface Task {
    id: string;
    text: string;
    type: string;
    answer: string | null;
}

export interface CreateTaskDto {
    text: string;
    type: number;
    answer?: string;
}

export interface UpdateTaskDto {
    id: number;
    text: string;
    type: number;
    answer?: string;
}
