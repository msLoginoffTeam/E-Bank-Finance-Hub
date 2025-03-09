import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import {JSX} from "react";
import {useAppSelector} from "../hooks/redux.ts";
import {LoginPage} from "../pages/LoginPage.tsx";
import {RegisterPage} from "../pages/RegisterPage.tsx";
import {DashboardPage} from "../pages/DashboardPage.tsx";
import {AccountsPage} from "../pages/AccountsPage.tsx";
import {AccountDetailsPage} from "../pages/AccountDetailsPage.tsx";
import {CreditDetailsPage} from "../pages/CreditDetailsPage.tsx";
import {CreditsPage} from "../pages/CreditsPage.tsx";

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

            <Route
                path="/accounts"
                element={
                    <PrivateRoute>
                        <AccountsPage />
                    </PrivateRoute>
                }
            />
            <Route path="/accounts/:accountId" element={
                <PrivateRoute>
                    <AccountDetailsPage/>
                </PrivateRoute>
            } />
            <Route path="/credits" element={
                <PrivateRoute>
                    <CreditsPage></CreditsPage>
                </PrivateRoute>
            } />

            <Route path="/credits/:creditId" element={
                <PrivateRoute>
                   <CreditDetailsPage></CreditDetailsPage>
                </PrivateRoute>
            } />

            {/* Заглушка на несуществующие маршруты */}
            <Route path="*" element={<Navigate to="/" />} />
        </Routes>
    </BrowserRouter>
);
