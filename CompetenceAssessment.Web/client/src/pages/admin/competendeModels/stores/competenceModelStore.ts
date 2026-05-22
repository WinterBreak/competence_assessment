import {makeAutoObservable, runInAction } from 'mobx';
import { CompetenceModel, CreateCompetenceModelDto, UpdateCompetenceModelDto } from '../types/model.types';
import { competenceModelService } from '../services/competenceModelService';

export class CompetenceModelStore {
    competenceModels: CompetenceModel[] = [];
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
        this.loadCompetenceModels();
    }

    setCurrentPage(page: number) {
        this.currentPage = page;
        this.loadCompetenceModels();
    }

    setPageSize(size: number) {
        this.pageSize = size;
        this.currentPage = 1;
        this.loadCompetenceModels();
    }

    // API вызовы с бизнес-логикой
    async loadCompetenceModels() {
        this.isLoading = true;
        this.error = null;

        try {
            const response = await competenceModelService.getCompetenceModels({
                page: this.currentPage,
                pageSize: this.pageSize,
            });

            runInAction(() => {
                this.competenceModels = response;
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

    async createCompetenceModel(data: CreateCompetenceModelDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await competenceModelService.createCompetenceModel(data);
            runInAction(async () => {
                if (result.hasErrors){
                    await this.loadCompetenceModels();
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

    async updateCompetenceModel(data: UpdateCompetenceModelDto): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await competenceModelService.updateCompetenceModel(data);
            runInAction( async () => {
                if (result.hasErrors){
                    await this.loadCompetenceModels();
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

    async deleteCompetenceModel(id: string): Promise<boolean> {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await competenceModelService.deleteCompetenceModel(id);
            runInAction(async () => {
                if (result.hasErrors){
                    await this.loadCompetenceModels();
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

    async deleteManyCompetenceModels(ids: string[]): Promise<boolean> {
        if (ids.length === 0) return false;

        this.isLoading = true;
        this.error = null;

        try {
            await competenceModelService.deleteManyCompetenceModels(ids);
            runInAction(() => {
                this.competenceModels = this.competenceModels.filter(
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

    async exportCompetenceModels(): Promise<void> {
        this.isLoading = true;
        this.error = null;

        try {
            const blob = await competenceModelService.exportCompetenceModels();
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `competence_models_${new Date().toISOString()}.xlsx`;
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

export const competenceModelStore = new CompetenceModelStore();