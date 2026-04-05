import {Competence} from "../../competencies/types/competence.types";

export interface CompetenceModel {
    id: string;
    name: string;
    description: string | null;
    creationDate: Date;
    competencies: CompetenceModelWeight[];
}

export interface CompetenceModelWeight {
    model_id: string;
    competence: Competence;
    weight: number;
}

export interface CreateCompetenceModelDto {
    name: string;
    description: string;
}

export interface UpdateCompetenceModelDto {
    id: number;
    name?: string;
    description?: string;
}