import { useAccountsQuery } from '../queries/accounts.queries';
import { Card, Center, Container, Loader, Stack, Text, Title } from '@mantine/core';

export const AccountsPage = () => {
    const { data: accounts, isLoading, error } = useAccountsQuery();
    //const { mutate: openAccount, isPending: isOpening } = useOpenAccountMutation();

    return (
        <Container size="md" py="xl">
            <Title mb="lg">Мои счета</Title>

            {/*<Center mb="md">*/}
            {/*    <Button onClick={() => openAccount()} loading={isOpening}>*/}
            {/*        Открыть новый счёт*/}
            {/*    </Button>*/}
            {/*</Center>*/}

            {isLoading ? (
                <Center><Loader /></Center>
            ) : error ? (
                <Text color="red">Не удалось загрузить счета</Text>
            ) : accounts && accounts.length > 0 ? (
                <Stack>
                    {accounts.map((account: any) => (
                        <Card key={account.id} shadow="md" p="md">
                            <Text>Номер счёта: {account.id}</Text>
                            <Text>Баланс: {account.balanceInRubles} ₽</Text>
                            <Text>Название: {account.name}</Text>
                        </Card>
                    ))}
                </Stack>
            ) : (
                <Text>У вас пока нет счетов</Text>
            )}
        </Container>
    );
};
