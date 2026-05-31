import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    Assessment, AssessmentCalcDto, AssessmentCalculation,
    CreateAssessmentDto, EmployeeData, PositionData,
    UpdateAssessmentDto, CompetenceDevelopmentDto, DepartmentData, ExportReportRequest
} from '../types/assessment.types';
import {ApiResponse, PaginatedResponse} from "../../../types/common.types";

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
    }): Promise<PaginatedResponse<Assessment>> {
        try {
            const response = await this.api.get('/assessments', {
                params: {
                    page: params?.page || 1,
                    pageSize: params?.pageSize || 10,
                }
            });
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

    async exportAssessments(request: ExportReportRequest): Promise<void> {
        try {
            const response = await this.api.post(
                '/assessments/export',
                request,
                {
                    responseType: 'blob',
                }
            );

            const blob = new Blob(
                [response.data],
                {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                }
            );
            
            const contentDisposition = response.headers['content-disposition'];
            let fileName = 'report.xlsx';
            const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/);
            if (utf8Match?.[1]) {
                fileName = decodeURIComponent(utf8Match[1]);
            } else {
                const asciiMatch = contentDisposition.match(/filename="?([^"]+)"?/);

                if (asciiMatch?.[1]) {
                    fileName = asciiMatch[1];
                }
            }

            const url = window.URL.createObjectURL(response.data);

            const link = document.createElement('a');

            link.href = url;
            link.download = fileName;

            document.body.appendChild(link);

            link.click();

            link.remove();

            window.URL.revokeObjectURL(url);
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