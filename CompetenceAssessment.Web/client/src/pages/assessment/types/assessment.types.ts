import {AssessmentParticipant} from "../../admin/users/types/user.types";
import {Answer} from "../../admin/tasks/types/task.types";

export interface Assessment {
    id: string;
    candidate: AssessmentParticipant;
    startDate: Date;
    endDate: Date;
    type: string;
    template: AssessmentTemplate;
    inspectors: AssessmentParticipant[];
    results: AssessmentResult[];
    state: number; // TODO нужна статусная модель
}

export interface AssessmentTemplate {
    id: string;
    type: string;
    name: string;
    scale: number;
    tasks: AssessmentTask[];
}

export interface AssessmentTask {
    id: string;
    type: string;
    text: string;
    answers: Answer[];
}

export interface AssessmentResult {
    id: string;
    assessmentId: string;
    taskId: string;
    comment?: string;
    answer: string;
    score: number;
}

export interface AssessmentCalculation {
    referenceTotal: number;
    receivedTotal: number;
    totalReceivedPercentage: number;
    competenceReferences: Record<number, number>;
    competenciesReceived: Record<number, number>;
    competenciesReceivedPercentage: Record<number, number>;
    competenceNames: Record<number, string>;
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

export interface DevelopmentPointDto {
    date: Date;
    score: number;
    competenceId: string;
    competenceName: string;
}

export interface CompetenceDevelopmentDto {
    employeeId: number;
    employeeName: string;
    targetScore: number;
    developmentHistory: DevelopmentPointDto[];
}

export interface AssessmentCalcDto {
    id: string;
}

export interface EmployeeData {
    userId: string;
    fullName: string;
    department: string;
    position: string;
    competencies: EmployeeCompetenceData[];
}

export interface EmployeeCompetenceData {
    competenceId: string;
    name: string;
    score: number;
    percentage: number;
    level: string;
}

export interface PositionData {
    positionId: string;
    positionName: string;
    score: number;
    percentage: number;
    level: string;
    competencies: CompetenceData[];
    employees: AssessmentParticipant[];
}

export interface CompetenceData {
    competenceId: string;
    competenceName: string;
    score: number;
    percentage: number;
    level: string;
}

export interface DepartmentData {
    departmentId: string;
    departmentName: string;
    score: number;
    percentage: number;
    level: string;
    competencies: CompetenceData[];
    employees: AssessmentParticipant[];
}