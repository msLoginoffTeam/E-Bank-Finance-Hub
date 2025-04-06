import { configureStore } from '@reduxjs/toolkit';

import { AccountsReducer } from './AccountsStore';
import { AppReducer } from './AppStore';
import { AuthReducer } from './AuthStore';
import { ClientsReducer } from './ClientsStore';
import { CreditsReducer } from './CreditsStore';

export const store = configureStore({
  reducer: {
    accounts: AccountsReducer,
    auth: AuthReducer,
    clients: ClientsReducer,
    credits: CreditsReducer,
    app: AppReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
