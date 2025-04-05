import { useParams, useNavigate } from 'react-router-dom';
import {useAccountsQuery, useCloseAccountMutation} from '../queries/accounts.queries';
import {Card, Stack, Title, Text, Button, Group, Modal, ScrollArea} from '@mantine/core';
import { useState, useMemo } from 'react';
import {useOperationsQuery} from "../queries/operations.queries.ts";

export const AccountDetailsPage = () => {
    const { accountId } = useParams();
    const navigate = useNavigate();
    const { data: accounts, isLoading: isAccountsLoading } = useAccountsQuery();
    const { data: operations, isLoading: isOperationsLoading } = useOperationsQuery(accountId!);
    const closeAccountMutation = useCloseAccountMutation(accountId!);

    const [isCloseModalOpen, setIsCloseModalOpen] = useState(false);

    // Находим счет из общего списка
    const account = useMemo(() => accounts?.find((acc : any) => acc.id === accountId), [accounts, accountId]);

    // Если счета нет, редиректим на дашборд
    if (isAccountsLoading || isOperationsLoading) return <p>Загрузка...</p>;
    if (!account) {
        navigate('/dashboard');
        return null;
    }

    // Закрытие счета с подтверждением
    const handleCloseAccount = async () => {
        await closeAccountMutation.mutateAsync();
        setIsCloseModalOpen(false);
    };

    return (
        <Stack>
            {/* Информация о счете */}
            <Card shadow="lg" p="md">
                <Title order={3}>Счет № {account.id}</Title>
                <Text>Баланс: {account.balanceInRubles} ₽</Text>
                <Text color={!account.isClosed ? 'green' : 'red'}>
                    {!account.isClosed ? 'Открыт' : 'Закрыт'}
                </Text>

                {!account.isClosed && (
                    <Button color="red" mt="md" onClick={() => setIsCloseModalOpen(true)}>
                        Закрыть счет
                    </Button>
                )}
            </Card>

            {/* История операций */}
            <Card shadow="lg" p="md" style={{ flex: 1 }}>
                <Title order={3}>История операций</Title>
                <ScrollArea style={{ height: "75vh" }}>
                <Stack >
                    {operations.map((operation : any) => (
                        <Group key={operation.id}>
                            <span>{operation.operationType === 'Income' ? 'Пополнение' : 'Списание'}</span>
                            <span>{operation.amountInRubles}₽</span>
                        </Group>
                    ))}
                </Stack>
                </ScrollArea>
            </Card>

            {/* Модалка подтверждения закрытия счета */}
            <Modal opened={isCloseModalOpen} onClose={() => setIsCloseModalOpen(false)} title="Подтвердите действие">
                <Text>Вы уверены, что хотите закрыть этот счет? Это действие нельзя отменить.</Text>
                <Group mt="md">
                    <Button color="red" onClick={handleCloseAccount} loading={closeAccountMutation.isPending}>
                        Закрыть счет
                    </Button>
                    <Button variant="outline" onClick={() => setIsCloseModalOpen(false)}>Отмена</Button>
                </Group>
            </Modal>
        </Stack>
    );
};
