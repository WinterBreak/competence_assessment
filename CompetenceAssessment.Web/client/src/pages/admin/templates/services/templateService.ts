import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    Template,
    CreateTemplateDto,
    UpdateTemplateDto,
} from '../types/template.types';
import {ApiResponse} from "../../../../types/common.types";

class TemplateService {
    private api: AxiosInstance;

    constructor() {
        this.api = axios.create({
            baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5173/api',
            headers: {
                'Content-Type': 'application/json',
            },
            timeout: 100000,
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
    
    async getTemplates(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<Template[]> {
        try {
            const response = await this.api.get('/templates');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async getTemplateById(id: string): Promise<Template> {
        try {
            const response = await this.api.get(`/templates/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async createTemplate(data: CreateTemplateDto): Promise<ApiResponse> {
        try {
            const response = await this.api.post('/templates', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async updateTemplate(data: UpdateTemplateDto): Promise<ApiResponse> {
        try {
            const response = await this.api.patch(`/templates`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async deleteTemplate(id: string): Promise<ApiResponse> {
        try {
            const response = await this.api.delete(`/templates/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async deleteManyTemplates(ids: string[]): Promise<void> {
        try {
            await this.api.post('/templates/delete-many', { ids });
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async exportTemplates(): Promise<Blob> {
        try {
            const response = await this.api.get('/templates/export', {
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

export const templateService = new TemplateService();