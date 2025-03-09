import { UserRole } from '~/store/AuthStore';

export const LOGIN = '/api/users/login';

// http://147.45.158.33:8082/api/users/create?Role=Employee
export const REGISTER = (role: UserRole) => `/api/users/create?Role=${role}`;
export const GET_EMPLOYEE_PROFILE = '/api/users/api/client/profile';
export const BLOCK_USER = (id: string) => `/api/users/block/${id}`;
export const UNBLOCK_USER = (id: string) => `/api/users/unblock/${id}`;
