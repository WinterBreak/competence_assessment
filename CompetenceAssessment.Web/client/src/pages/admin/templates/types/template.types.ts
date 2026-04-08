import { CompetenceModel } from "../../competendeModels/types/model.types";
import {Task} from "../../tasks/types/task.types";

export interface Template {
    id: string;
    competenceModel: TemplateCompetenceModel;
    name: string;
    type: string;
    scale: number;
    creationDate: Date;
    tasks: TemplateWeight[];
}

export interface TemplateWeight {
    templateId: string;
    taskId: string;
    taskText: string;
    competenceId: string;
    weight: number;
}

export interface TemplateCompetenceModel {
    modelId: string;
    name: string;
    description?: string;
    competencies: Record<string, string>;
}

export interface CreateTemplateDto {
    name: string;
    type: string;
    scale: number;
    competenceModelId: string;
    weights: Record<number, number>;
    competenciesToTasks: Record<number, number[]>;
}

export interface UpdateTemplateDto {
    id: string;
    name: string;
    type: string;
    scale: number;
    competenceModelId: string;
    weights: Record<number, number>;
    competenciesToTasks: Record<number, number[]>;
}