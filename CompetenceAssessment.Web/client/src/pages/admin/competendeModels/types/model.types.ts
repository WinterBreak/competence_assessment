import {Competence} from "../../competencies/types/competence.types";

export interface CompetenceModel {
    id: string;
    name: string;
    description?: string;
    creationDate: Date;
    competencies: ModelCompetence[];
}

export interface ModelCompetence {
    modelId: string;
    competenceId: string;
    name: string;
    description?: string;
    weight: number;
}

export interface CreateCompetenceModelDto {
    name: string;
    description?: string;
    weights: Record<string, number>;
}

export interface UpdateCompetenceModelDto {
    id: number;
    name: string;
    description?: string;
    weights?: Record<string, number>;
}