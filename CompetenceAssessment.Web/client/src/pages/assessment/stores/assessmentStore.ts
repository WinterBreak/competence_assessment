import {makeAutoObservable, runInAction } from 'mobx';
import { Assessment, CreateAssessmentDto, UpdateAssessmentDto } from '../types/assessment.types';
import { assessmentService } from '../services/assessmentService';

export class AssessmentStore {
    assessments: Assessment[] = [];
    currentAssessment?: Assessment;
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
        this.loadAssessments();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadAssessments();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadAssessments();
    }

    // API вызовы с бизнес-логикой
    async loadAssessmentById(id: string) {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await assessmentService.getAssessmentById(id);

            runInAction(() => {
                this.currentAssessment = response;
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
    
    async loadAssessments() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await assessmentService.getAssessments({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.assessments = response;
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

    async createAssessment(data: CreateAssessmentDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await assessmentService.createAssessment(data);
            runInAction( async () => {
                if (!result.hasErrors){
                    await this.loadAssessments();
                }
                else{
                    this.error = result.errorValues == undefined
                        ? 'Неизвестная ошибка'
                        : Object.entries(result.errorValues)
                            .map(([key, value]) => `${key}: ${value}`)
                            .join('\n');
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

    async updateAssessment(data: UpdateAssessmentDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await assessmentService.updateAssessment(data);
            runInAction( async() => {
                if (result.hasErrors){
                    await this.loadAssessments();
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

    async deleteAssessment(id: string): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await assessmentService.deleteAssessment(id);
            runInAction( async () => {
                if (result.hasErrors){
                    await this.loadAssessments();
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

    async deleteManyAssessments(ids: string[]): Promise<boolean> {
        if (ids.length === 0) return false;

        this.isLoading = true;
        this.error = null;

        try {
            await assessmentService.deleteManyAssessments(ids);
            runInAction(() => {
                this.assessments = this.assessments.filter(
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

    async exportAssessments(): Promise<void> {
        this.isLoading = true;
        this.error = null;

        try {
            const blob = await assessmentService.exportAssessments();
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `assessments_${new Date().toISOString()}.xlsx`;
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

export const assessmentStore = new AssessmentStore();