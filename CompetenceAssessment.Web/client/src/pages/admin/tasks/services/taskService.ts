import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    Task,
    CreateTaskDto,
    UpdateTaskDto,
} from '../types/task.types';
import {ApiResponse, PaginatedResponse} from "../../../../types/common.types";

class TaskService {
    private api: AxiosInstance;

    constructor() {
        // Базовый URL из переменных окружения
        this.api = axios.create({
            baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5173/api',
            headers: {
                'Content-Type': 'application/json',
            },
            timeout: 100000, // 10 секунд таймаут
        });

        // Интерсептор для добавления токена авторизации
        // this.api.interceptors.request.use(
        //     (config) => {
        //         const token = localStorage.getItem('accessToken');
        //         if (token) {
        //             config.headers.Authorization = `Bearer ${token}`;
        //         }
        //         return config;
        //     },
        //     (error) => Promise.reject(error)
        // );
        //
        // // Интерсептор для обработки ошибок
        // this.api.interceptors.response.use(
        //     (response) => response,
        //     (error) => {
        //         if (error.response?.status === 401) {
        //             // Неавторизован - перенаправляем на логин
        //             window.location.href = '/login';
        //         }
        //         return Promise.reject(error);
        //     }
        // );
    }

    async getTasks(params?: {
        page?: number;
        pageSize?: number;
        search?: string;
    }): Promise<PaginatedResponse<Task>> {
        try {
            const response = await this.api.get('/tasks', {
                params: {
                    page: params?.page || 1,
                    pageSize: params?.pageSize || 10,
                    searchTerm: params?.search || ''
                }
            });
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async getTaskById(id: string): Promise<Task> {
        try {
            const response = await this.api.get(`/tasks/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async createTask(data: CreateTaskDto): Promise<ApiResponse> {
        try {
            const response = await this.api.post('/tasks', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async updateTask(data: UpdateTaskDto): Promise<ApiResponse> {
        try {
            const response = await this.api.patch(`/tasks`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async deleteTask(id: string): Promise<void> {
        try {
            await this.api.delete(`/tasks/${id}`);
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async deleteManyTasks(ids: string[]): Promise<void> {
        try {
            await this.api.post('/tasks/delete-many', { ids });
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async exportTasks(): Promise<Blob> {
        try {
            const response = await this.api.get('/tasks/export', {
                responseType: 'blob',
            });
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    private handleError(error: unknown): Error {
        if (error instanceof AxiosError) {
            const message = error.response?.data?.message || error.message;
            const status = error.response?.status;
            return new Error(`[${status}] ${message}`);
        }
        return new Error('Неизвестная ошибка');
    }
}

export const taskService = new TaskService();