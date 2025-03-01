export interface ClientsState {
  clients: Client[];
  isLoading: boolean;
  error?: string;
}

export interface Client {
  id: string;
  email: string;
  fullName: string;
  isBlocked: boolean;
}
