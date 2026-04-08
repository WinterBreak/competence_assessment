import {makeAutoObservable, runInAction } from 'mobx';
import { Template, CreateTemplateDto, UpdateTemplateDto } from '../types/template.types';
import { templateService } from '../services/templateService';

export class TemplateStore {
    templates: Template[] = [];
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
        this.loadTemplates();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadTemplates();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadTemplates();
    }

    // API вызовы с бизнес-логикой
    async loadTemplates() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await templateService.getTemplates({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.templates = response;
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

    async createTemplate(data: CreateTemplateDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await templateService.createTemplate(data);
            runInAction( async () => {
                if (result.hasErrors){
                    await this.loadTemplates();
                }
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

    async updateTemplate(data: UpdateTemplateDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await templateService.updateTemplate(data);
            runInAction( async() => {
                if (result.hasErrors){
                    await this.loadTemplates();
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

    async deleteTemplate(id: string): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await templateService.deleteTemplate(id);
            runInAction( async () => {
                if (result.hasErrors){
                    await this.loadTemplates();
                }
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

    async deleteManyTemplates(ids: string[]): Promise<boolean> {
        if (ids.length === 0) return false;

        this.isLoading = true;
        this.error = null;

        try {
            await templateService.deleteManyTemplates(ids);
            runInAction(() => {
                this.templates = this.templates.filter(
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

    async exportTemplates(): Promise<void> {
        this.isLoading = true;
        this.error = null;

        try {
            const blob = await templateService.exportTemplates();
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `templates_${new Date().toISOString()}.xlsx`;
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

export const templateStore = new TemplateStore();