import {makeAutoObservable, runInAction } from 'mobx';
import { User } from '../types/user.types';
import { userService } from '../services/userService';

export class UserStore {
    users: User[] = [];
    isLoading: boolean = false;
    error: string | null = null;

    currentPage: number = 1;
    pageSize: number = 10;
    totalItems: number = 0;
    totalPages: number = 0;

    searchTerm: string = '';

    constructor() {
        makeAutoObservable(this);
    }

    setSearchTerm(term: string) {
        this.searchTerm = term;
        this.currentPage = 1;
        this.loadUsers();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadUsers();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadUsers();
    }
    
    async loadUsers() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await userService.getUsers({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.users = response.items;
                this.totalItems = response.totalCount;
                this.totalPages = response.totalPages;
                this.isLoading = false;
            });
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка загрузки данных';
                this.isLoading = false;
            });
        }
    }

    clearError() {
        this.error = null;
    }
}

export const userStore = new UserStore();