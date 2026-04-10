import {AssessmentParticipant} from "../../admin/users/types/user.types";
import {Template} from "../../admin/templates/types/template.types";

export interface Assessment {
    id: string;
    candidate: AssessmentParticipant;
    startDate: Date;
    endDate: Date;
    type: string;
    template: Template;
    inspectors: AssessmentParticipant[];
    results: AssessmentResult[];
    isFinished: boolean; // TODO нужна статусная модель
}

export interface AssessmentResult {
    id: string;
    assessmentId: string;
    taskId: string;
    comment?: string;
    answer: string;
    score: number;
}

export interface CreateAssessmentDto {
    templateId: number;
    type: number;
    candidateId: number;
    inspectorsIds: number[];
}

export interface UpdateAssessmentDto {
    assessmentId: string;
    answers: Record<string, string>;
    scores: Record<string, number>;
    comments: Record<string, string>;
    comment: string;
}