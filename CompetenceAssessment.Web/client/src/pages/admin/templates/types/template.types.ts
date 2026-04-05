import { CompetenceModel } from "../../competendeModels/types/model.types";
import {Task} from "../../tasks/types/task.types";

export interface Template {
    id: string;
    competenceModelId: number;
    name: string;
    type: string;
    scale: string;
    creationDate: Date;
    tasks: TemplateWeight[];
}

export interface TemplateWeight {
    template_id: number;
    task: Task;
    competence_id: number;
    weight: number;
}

export interface CreateTemplateDto {
    name: string;
    description: string;
}

export interface UpdateTemplateDto {
    id: number;
    name?: string;
    description?: string;
}