import {makeAutoObservable, runInAction } from 'mobx';
import { Task, CreateTaskDto, UpdateTaskDto } from '../types/task.types';
import { taskService } from '../services/taskService';

export class TaskStore {
    tasks: Task[] = [];
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
        this.loadTasks();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadTasks();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadTasks();
    }

    // API вызовы с бизнес-логикой
    async loadTasks() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await taskService.getTasks({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.tasks = response;
                // this.totalItems = response.total;
                // this.totalPages = response.totalPages;
                this.isLoading = false;
            });
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка загрузки данных';
                this.isLoading = false;
            });
        }
    }

    async createTask(data: CreateTaskDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const newTask = await taskService.createTask(data);
            runInAction(() => {
                this.tasks.unshift(newTask);
                this.totalItems++;
                this.isLoading = false;
            });
            return true;
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка создания';
                this.isLoading = false;
            });
            return false;
        }
    }

    async updateTask(id: string, data: UpdateTaskDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const updated = await taskService.updateTask(id, data);
            runInAction(() => {
                const index = this.tasks.findIndex(c => c.id === id);
                if (index !== -1) {
                    this.tasks[index] = updated;
                }
                this.isLoading = false;
            });
            return true;
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка обновления';
                this.isLoading = false;
            });
            return false;
        }
    }

    async deleteTask(id: string): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            await taskService.deleteTask(id);
            runInAction(() => {
                this.tasks = this.tasks.filter(c => c.id !== id);
                this.totalItems--;
                this.isLoading = false;
            });
            return true;
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка удаления';
                this.isLoading = false;
            });
            return false;
        }
    }

    async deleteManyTasks(ids: string[]): Promise<boolean> {
        if (ids.length === 0) return false;

        this.isLoading = true;
        this.error = null;

        try {
            await taskService.deleteManyTasks(ids);
            runInAction(() => {
                this.tasks = this.tasks.filter(
                    c => !ids.includes(c.id)
                );
                this.totalItems -= ids.length;
                this.isLoading = false;
            });
            return true;
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка массового удаления';
                this.isLoading = false;
            });
            return false;
        }
    }

    async exportTasks(): Promise<void> {
        this.isLoading = true;
        this.error = null;

        try {
            const blob = await taskService.exportTasks();
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `tasks_${new Date().toISOString()}.xlsx`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);

            runInAction(() => {
                this.isLoading = false;
            });
        } catch (error) {
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Ошибка экспорта';
                this.isLoading = false;
            });
        }
    }

    // Утилиты
    clearError() {
        this.error = null;
    }
}

export const taskStore = new TaskStore();