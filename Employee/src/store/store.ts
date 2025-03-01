import { configureStore } from '@reduxjs/toolkit';

import { AccountsReducer } from './AccountsStore';
import { AuthReducer } from './AuthStore';
import { ClientsReducer } from './ClientsStore';

export const store = configureStore({
  reducer: {
    accounts: AccountsReducer,
    auth: AuthReducer,
    clients: ClientsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
