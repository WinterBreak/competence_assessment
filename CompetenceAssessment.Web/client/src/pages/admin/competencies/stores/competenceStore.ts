import {makeAutoObservable, runInAction } from 'mobx';
import { Competence, CreateCompetencyDto, UpdateCompetencyDto } from '../types/competence.types';
import { competenceService } from '../services/competenceService';

export class CompetenceStore {
    competencies: Competence[] = [];
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
        this.loadCompetencies();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadCompetencies();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadCompetencies();
    }

    // API вызовы с бизнес-логикой
    async loadCompetencies() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await competenceService.getCompetencies({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.competencies = response;
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

    async createCompetency(data: CreateCompetencyDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const newCompetency = await competenceService.createCompetency(data);
            runInAction(async () => {
                await this.loadCompetencies();
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

    async updateCompetency(data: UpdateCompetencyDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const updated = await competenceService.updateCompetency(data);
            runInAction(async () => {
                if (!updated.hasErrors) {
                    await this.loadCompetencies(); // TODO подумать, как лучше это сделать
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

    async deleteCompetency(id: string): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            await competenceService.deleteCompetency(Number(id));
            runInAction( async () => {
                await this.loadCompetencies();
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

    async deleteManyCompetencies(ids: string[]): Promise<boolean> {
        if (ids.length === 0) return false;

        this.isLoading = true;
        this.error = null;

        try {
            await competenceService.deleteManyCompetencies(ids);
            runInAction(() => {
                this.competencies = this.competencies.filter(
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

    async exportCompetencies(): Promise<void> {
        this.isLoading = true;
        this.error = null;

        try {
            const blob = await competenceService.exportCompetencies();
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `competencies_${new Date().toISOString()}.xlsx`;
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

export const competenceStore = new CompetenceStore();