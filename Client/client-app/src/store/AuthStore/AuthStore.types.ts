export interface AuthState {
    token: string | null;
    error: string | null;
    loading: boolean;
}

export interface Credentials {
    email: string;
    password: string;
}

export interface RegisterData extends Credentials {
    fullName: string;
}