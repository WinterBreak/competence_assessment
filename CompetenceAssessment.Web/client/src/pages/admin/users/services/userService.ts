import axios, { AxiosError, AxiosInstance } from 'axios';
import {
    User
} from '../types/user.types';
import {ApiResponse, PaginatedResponse} from "../../../../types/common.types";

class UserService {
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

    async getUsers(params?: {
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedResponse<User>> {
        try {
            const response = await this.api.get('/users',
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

    async getUserById(id: string): Promise<User> {
        try {
            const response = await this.api.get(`/users/${id}`);
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

export const userService = new UserService();