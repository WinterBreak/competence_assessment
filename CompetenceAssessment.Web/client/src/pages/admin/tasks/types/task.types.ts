export interface Task {
    id: string;
    text: string;
    type: string;
    answers?: Answer[] | null;
}

export interface Answer {
    id: string;
    taskId: string;
    text: string;
    isCorrect: boolean;
}

export interface CreateTaskDto {
    text: string;
    type: number;
    answers?: Record<string, boolean>;
}

export interface UpdateTaskDto {
    id: number;
    text: string;
    type: number;
    answers?: Record<string, boolean>;
}
