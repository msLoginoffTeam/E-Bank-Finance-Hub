export enum Theme {
  LIGHT = 'LIGHT',
  DARK = 'DARK',
}

export interface AppState {
  theme: Theme;
  hiddenAccounts: string[];
  isLoading: boolean;
  error?: string;
}

export interface SettingsResponse {
  id: string;
  userId: string;
  theme: Theme;
  hiddenAccounts: string[];
}

export interface SettingsEdit {
  theme: Theme;
  hiddenAccounts: string[];
}
