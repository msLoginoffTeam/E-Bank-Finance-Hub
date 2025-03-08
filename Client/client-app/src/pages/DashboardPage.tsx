import { Button, Container, Grid, Title, Card, Stack } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../hooks/redux';
import { logout } from '../store/AuthStore';
import { useDashboardData } from '../hooks/useDashBoardData';
import { DashboardAccounts } from '../components/dashboard/DashboardAccounts';
import { DashboardOperations } from '../components/dashboard/DashboardOperations';

export const DashboardPage = () => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const token = useAppSelector((state) => state.auth.token);

    // Проверяем авторизацию
    useEffect(() => {
        if (!token) navigate('/login');
    }, [token]);

    // Функция выхода
    const handleLogout = () => {
        dispatch(logout());
    };

    // Загружаем данные
    const { accounts, operations, isLoading } = useDashboardData();

    if (isLoading) return <p>Загрузка...</p>;

    return (
        <Container size="xl" py="xl">
            <Grid gutter="lg">
                {/* Левая панель (Счета) */}
                <Grid.Col span={3}>
                    <DashboardAccounts accounts={accounts} />
                </Grid.Col>

                {/* Центральный блок (Действия) */}
                <Grid.Col span={6}>
                    <Card shadow="lg" p="md">
                        <Title order={3} mb="sm">Действия</Title>
                        <Stack>
                            <Button variant="outline">Пополнить</Button>
                            <Button variant="outline">Перевести</Button>
                            <Button variant="outline">Оплатить кредит</Button>
                        </Stack>
                    </Card>
                </Grid.Col>

                {/* Правая панель (История операций) */}
                <Grid.Col span={3}>
                    <DashboardOperations operations={operations} />
                </Grid.Col>
            </Grid>

            {/* Кнопка выхода */}
            <Button color="red" onClick={handleLogout} fullWidth mt="lg">
                Выйти
            </Button>
        </Container>
    );
};
