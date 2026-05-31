export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    userId: string;
    email: string;
    fullName: string;
    roles: string;
}