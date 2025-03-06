import { Button } from '@mantine/core';
import {useAppDispatch, useAppSelector} from "../hooks/redux.ts";
import {logout} from "../store/AuthStore";
import {useNavigate} from "react-router-dom";
import {useEffect} from "react";

export const DashboardPage = () => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const token = useAppSelector((state) => state.auth.token);

    const handleLogout = () => {
        dispatch(logout());
    };

    useEffect(() => {
        if (!token) navigate('/login');
    }, [token]);

    return (
        <div style={{ padding: '2rem' }}>
            <h1>Добро пожаловать в интернет-банк!</h1>
            <p>Здесь будет главная информация о счетах, кредитах и истории операций.</p>
            <Button color="red" onClick={handleLogout}>
                Выйти
            </Button>
        </div>
    );
};