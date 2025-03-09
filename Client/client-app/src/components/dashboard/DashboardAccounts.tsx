import {Card, Stack, Button, Title, ActionIcon} from '@mantine/core';
import {AccountCard} from "../accounts/AccountCard.tsx";
import {Link} from "react-router-dom";

export const DashboardAccounts = ({ accounts }: { accounts: any[] }) => {
    // Сортируем: сначала активные, потом закрытые

    return (
        <Card shadow="lg" p="md">
            <Title order={3} mb="sm" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                Мои счета
                <ActionIcon variant="light" size="lg">
                    {/*<IconPlus size={20} />*/}
                </ActionIcon>
            </Title>
            <Stack>
                {accounts && accounts.map((account) => (
                    <AccountCard key={account.id} account={account}/>
                ))}
                <Button fullWidth mt="sm" component={Link} to="/accounts">Посмотреть все</Button>
            </Stack>
        </Card>
    );
};
