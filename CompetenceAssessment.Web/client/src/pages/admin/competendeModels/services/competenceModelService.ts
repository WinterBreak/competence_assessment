import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    CompetenceModel,
    CreateCompetenceModelDto,
    UpdateCompetenceModelDto,
} from '../types/model.types';
import {ApiResponse, PaginatedResponse} from "../../../../types/common.types";

class CompetenceModelService {
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

    // Получение списка компетенций с пагинацией
    async getCompetenceModels(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedResponse<CompetenceModel>> {
        try {
            const response = await this.api.get('/competence_models',
                {
                    params:{
                        page: params?.page || 1,
                        pageSize: params?.pageSize || 10,
                    }
                });
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Получение одной компетенции
    async getCompetenceModelById(id: string): Promise<CompetenceModel> {
        try {
            const response = await this.api.get(`/competence_models/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Создание компетенции
    async createCompetenceModel(data: CreateCompetenceModelDto): Promise<ApiResponse> {
        try {
            const response = await this.api.post('/competence_models', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Обновление компетенции
    async updateCompetenceModel(data: UpdateCompetenceModelDto): Promise<ApiResponse> {
        try {
            const response = await this.api.patch(`/competence_models`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Удаление компетенции
    async deleteCompetenceModel(id: string): Promise<ApiResponse> {
        try {
            const response = await this.api.delete(`/competence_models/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Массовое удаление
    async deleteManyCompetenceModels(ids: string[]): Promise<void> {
        try {
            await this.api.post('/competence_models/delete-many', { ids });
        } catch (error) {
            throw this.handleError(error);
        }
    }

    // Экспорт данных
    async exportCompetenceModels(): Promise<Blob> {
        try {
            const response = await this.api.get('/competence_models/export', {
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

export const competenceModelService = new CompetenceModelService();