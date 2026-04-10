export interface User {
    id: string;
    bossId?: string;
    bossName?: string;
    firstName: string;
    secondName?: string;
    lastName: string;
    email: string;
    positionId: string;
    position: string;
    departmentId: string;
    department: string;
}

export interface AssessmentParticipant {
    id: string;
    bossId?: string;
    fullName: string;
}