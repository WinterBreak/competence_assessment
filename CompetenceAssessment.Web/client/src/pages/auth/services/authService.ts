import axios, { AxiosError, AxiosInstance } from 'axios';
import { ApiResponse} from "../../../types/common.types";
import {LoginRequest, LoginResponse} from "../types/auth.types";

class AuthService {
    private api: AxiosInstance;

    constructor() {
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
    
    async login(data: LoginRequest): Promise<LoginResponse> {
        try {
            const response = await this.api.post('/auth/login', data);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    }
    
    async logout(): Promise<ApiResponse> {
        try {
            const response = await this.api.post('/auth/logout');
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

export const authService = new AuthService();