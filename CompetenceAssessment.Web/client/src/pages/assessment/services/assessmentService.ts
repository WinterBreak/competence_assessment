import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    Assessment, AssessmentCalcDto, AssessmentCalculation,
    CreateAssessmentDto, EmployeeData, PositionData,
    UpdateAssessmentDto, CompetenceDevelopmentDto, DepartmentData
} from '../types/assessment.types';
import {ApiResponse} from "../../../types/common.types";

class AssessmentService {
    private api: AxiosInstance;

    constructor() {
        this.api = axios.create({
            baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5173/api', // TODO вынести в отдельный файл вместе с другими url
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

    async getAssessments(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<Assessment[]> {
        try {
            const response = await this.api.get('/assessments');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getAssessmentHistory(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<Assessment[]> {
        try {
            const response = await this.api.get('/assessments/history');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getAssessmentById(id: string): Promise<Assessment> {
        try {
            const response = await this.api.get(`/assessments/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async createAssessment(data: CreateAssessmentDto): Promise<ApiResponse> {
        try {
            const response = await this.api.post('/assessments', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async updateAssessment(data: UpdateAssessmentDto): Promise<ApiResponse> {
        try {
            const response = await this.api.patch(`/assessments`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async deleteAssessment(id: string): Promise<ApiResponse> {
        try {
            const response = await this.api.delete(`/assessments/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async calculate(data: AssessmentCalcDto): Promise<AssessmentCalculation> {
        try {
            const response = await this.api.post(`/assessments/calculate`, data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getParticipantCompetencies(): Promise<EmployeeData[]> {
        try {
            const response = await this.api.get(`/assessments/participant_competencies`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getPositionCompetencies(): Promise<PositionData[]> {
        try {
            const response = await this.api.get(`/assessments/position_competencies`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getDepartmentsCompetencies(): Promise<DepartmentData[]> {
        try {
            const response = await this.api.get(`/assessments/department_competencies`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }

    async getCompetenceDevelopment (
        employeeId: string
    ): Promise<CompetenceDevelopmentDto> {
        const params: any = { employeeId };
        const response = await this.api.get(`/assessments/competence_development/${employeeId}`);
        return response.data;
    };

    async exportAssessments(): Promise<Blob> {
        try {
            const response = await this.api.get('/assessments/export', {
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

export const assessmentService = new AssessmentService();