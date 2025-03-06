import { configureStore } from '@reduxjs/toolkit';
import { AuthReducer } from './AuthStore';

export const store = configureStore({
    reducer: {
        auth: AuthReducer,
        // сюда позже добавим accounts и credits
    },
});

// Типизация для хуков
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;