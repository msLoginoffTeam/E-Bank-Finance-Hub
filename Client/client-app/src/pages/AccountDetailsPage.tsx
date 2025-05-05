import { useParams, useNavigate } from 'react-router-dom';
import {useAccountsQuery, useCloseAccountMutation} from '../queries/accounts.queries';
import {
    Card,
    Stack,
    Title,
    Text,
    Button,
    Group,
    Modal,
    ScrollArea,
} from '@mantine/core';
import { useState, useMemo } from 'react';
import {useOperationsQuery} from "../queries/operations.queries.ts";
import {currencyData, CurrencyEnum} from "../types/currency.ts";
import {useOperationsStream} from "../hooks/useOperationsStream.ts";

export const AccountDetailsPage = () => {
    const { accountId } = useParams();
    const navigate = useNavigate();
    const { data: accounts, isLoading: isAccountsLoading } = useAccountsQuery();
    //const { data: operations, isLoading: isOperationsLoading } = useOperationsQuery(accountId!);
    const closeAccountMutation = useCloseAccountMutation(accountId!);
    const { operations } =
        useOperationsStream(accountId!);

    const [isCloseModalOpen, setIsCloseModalOpen] = useState(false);

    const account = useMemo(() => accounts?.find((acc : any) => acc.id === accountId), [accounts, accountId]);

    if (isAccountsLoading) return <p>Загрузка...</p>;
    if (!account) {
        navigate('/dashboard');
        return null;
    }

    const handleCloseAccount = async () => {
        await closeAccountMutation.mutateAsync();
        setIsCloseModalOpen(false);
    };

    let currency;
    if (account.currency === currencyData[CurrencyEnum.Euro]) {
        currency = '€';
    }
    else if (account.currency === currencyData[CurrencyEnum.Ruble]) {
        currency = '₽'
    }
    else {
        currency = '$'
    }

    return (
        <Stack>
            <Card shadow="lg" p="md">
                <Title order={3}>Счет № {account.id}</Title>
                <Text>Баланс: {account.balance} {currency}</Text>
                <Text color={!account.isClosed ? 'green' : 'red'}>
                    {!account.isClosed ? 'Открыт' : 'Закрыт'}
                </Text>

                {!account.isClosed && (
                    <Button color="red" mt="md" onClick={() => setIsCloseModalOpen(true)}>
                        Закрыть счет
                    </Button>
                )}
            </Card>

            <Card shadow="lg" p="md" style={{ flex: 1 }}>
                <Title order={3}>История операций</Title>
                <ScrollArea style={{ height: "75vh" }}>
                <Stack >
                    {operations.map((operation : any) => (
                        <Group key={operation.id}>
                            <span>{operation.operationType === 'Income' ? 'Пополнение' : 'Списание'}</span>
                            <span>{operation.amount} {currency}</span>
                        </Group>
                    ))}
                </Stack>
                </ScrollArea>
            </Card>

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
