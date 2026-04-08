import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    Competence,
    CreateCompetencyDto,
    UpdateCompetencyDto,
    PaginatedResponse
} from '../types/competence.types';
import { ApiResponse} from "../../../../types/common.types";

class CompetenceService {
    private api: AxiosInstance;

    constructor() {
        // Базовый URL из переменных окружения
        this.api = axios.create({
            baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5173/api',
            headers: {
                'Content-Type': 'application/json',
            },
            timeout: 10000, // 10 секунд таймаут
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

    // Получение списка компетенций с пагинацией
    async getCompetencies(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<Competence[]> {
        try {
            const response = await this.api.get('/competencies');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Получение одной компетенции
    async getCompetencyById(id: string): Promise<Competence> {
        try {
            const response = await this.api.get(`/competencies/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Создание компетенции
    async createCompetency(data: CreateCompetencyDto): Promise<Competence> {
        try {
            const response = await this.api.post('/competencies', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Обновление компетенции
    async updateCompetency(data: UpdateCompetencyDto): Promise<ApiResponse> {
        try {
            const response = await this.api.patch(`/competencies`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Удаление компетенции
    async deleteCompetency(id: number): Promise<void> {
        try {
            await this.api.delete(`/competencies/${id}`);
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Массовое удаление
    async deleteManyCompetencies(ids: string[]): Promise<void> {
        try {
            await this.api.post('/competencies/delete-many', { ids });
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Экспорт данных
    async exportCompetencies(): Promise<Blob> {
        try {
            const response = await this.api.get('/competencies/export', {
                responseType: 'blob',
            });
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Обработка ошибок
    private handleError(error: unknown): Error {
        if (error instanceof AxiosError) {
            const message = error.response?.data?.message || error.message;
            const status = error.response?.status;
            return new Error(`[${status}] ${message}`);
        }
        return new Error('Неизвестная ошибка');
    }
}

export const competenceService = new CompetenceService();