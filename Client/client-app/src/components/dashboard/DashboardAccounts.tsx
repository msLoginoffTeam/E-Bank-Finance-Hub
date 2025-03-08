import { Card, Button, Title, ActionIcon } from '@mantine/core';
import { AccountList } from '../accounts/AccountList';
import { Link } from 'react-router-dom';

export const DashboardAccounts = ({ accounts }: { accounts: any[] }) => {
    return (
        <Card shadow="lg" p="md">
            <Title order={3} mb="sm" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                Мои счета
                <ActionIcon variant="light" size="lg">
                    {/*<IconPlus size={20} />*/}
                </ActionIcon>
            </Title>
            <AccountList accounts={accounts} />
            <Button fullWidth mt="md" component={Link} to="/accounts">
                Посмотреть все
            </Button>
        </Card>
    );
};
