import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import {JSX} from "react";
import {useAppSelector} from "../hooks/redux.ts";
import {LoginPage} from "../pages/LoginPage.tsx";
import {RegisterPage} from "../pages/RegisterPage.tsx";
import {DashboardPage} from "../pages/DashboardPage.tsx";

const PrivateRoute = ({ children }: { children: JSX.Element }) => {
    const token = useAppSelector((state) => state.auth.token);
    return token ? children : <Navigate to="/login" />;
};

export const AppRouter = () => (
    <BrowserRouter>
        <Routes>
            {/* Открытые страницы */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            {/* Закрытые страницы */}
            <Route
                path="/"
                element={
                    <PrivateRoute>
                        <DashboardPage />
                    </PrivateRoute>
                }
            />

            {/* Заглушка на несуществующие маршруты */}
            <Route path="*" element={<Navigate to="/" />} />
        </Routes>
    </BrowserRouter>
);
