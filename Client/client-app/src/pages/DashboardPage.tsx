import { Button, Container, Grid, Title, Card, Stack } from '@mantine/core';
import {Link, useNavigate} from 'react-router-dom';
import { useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../hooks/redux';
import { logout } from '../store/AuthStore';
import { useDashboardData } from '../hooks/useDashBoardData';
import { DashboardAccounts } from '../components/dashboard/DashboardAccounts';
import { DashboardOperations } from '../components/dashboard/DashboardOperations';
import {OpenAccountModal} from "../modals/OpenAccountModel.tsx";
import {useCreditsQuery} from "../queries/credits.queries.ts";
import {CreditCard} from "../components/credits/CreditCard.tsx";

export const DashboardPage = () => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const token = useAppSelector((state) => state.auth.token);
    const { data: credits } = useCreditsQuery();

    useEffect(() => {
        if (!token) navigate('/login');
    }, [token]);

    const [isOpenAccountModal, setIsOpenAccountModal] = useState(false);

    const { accounts, operations, isLoading } = useDashboardData();

    if (isLoading) return <p>Загрузка...</p>;

    console.log(credits)

    return (
        <Container size="xl" py="xl">
            <Grid gutter="lg">
                {/* Левая панель (Счета) */}
                <Grid.Col span={4}>
                    <DashboardAccounts accounts={accounts} />
                </Grid.Col>

                {/* Центральный блок (Действия) */}
                <Grid.Col span={4}>
                    <Card shadow="lg" p="md">
                        <Title order={3} mb="sm">Действия</Title>
                        <Stack>
                            <Button variant="outline" onClick={() => setIsOpenAccountModal(true)}>
                                Открыть счет
                            </Button>
                            <Button variant="outline">
                                Оформить кредит
                            </Button>
                        </Stack>
                    </Card>
                    <Card shadow="lg" p="md" mt="lg">
                        <Title order={3} mb="sm">Кредиты</Title>
                        <Stack>
                            {credits?.length ? <CreditCard credit={credits?.[0]} onPayOff={() => navigate(`/credits/${credits?.[0].id}`)}></CreditCard> : null}

                            <Button variant="outline" component={Link} to="/credits">
                                Все кредиты
                            </Button>
                        </Stack>
                    </Card>
                </Grid.Col>

                {/* Правая панель (История операций) */}
                <Grid.Col span={4}>
                    <DashboardOperations operations={operations} />
                </Grid.Col>
            </Grid>

            {/* Кнопка выхода */}
            <Button variant={'outline'} onClick={() => dispatch(logout())} fullWidth mt="lg">
                Выйти
            </Button>

            {/* Модалка открытия счета */}
            <OpenAccountModal opened={isOpenAccountModal} onClose={() => setIsOpenAccountModal(false)} />
        </Container>
    );
};
