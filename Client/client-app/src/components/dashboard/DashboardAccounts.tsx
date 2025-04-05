import {Card, Stack, Button, Title, ActionIcon, ScrollArea} from '@mantine/core';
import {AccountCard} from "../accounts/AccountCard.tsx";
import {Link} from "react-router-dom";

export const DashboardAccounts = ({ accounts, isExpanded }: { accounts: any[], isExpanded?: boolean }) => {
    return (
        <Card shadow="lg" p="md">
            <Title order={3} mb="sm" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                Мои счета
                <ActionIcon variant="light" size="lg">
                    {/*<IconPlus size={20} />*/}
                </ActionIcon>
            </Title>

            <Stack>
                <ScrollArea style={{ height: isExpanded ? "90vh" : undefined }}>
                {accounts && accounts.map((account) => (
                    <AccountCard key={account.id} account={account}/>
                ))}
            </ScrollArea>
            </Stack>

            {!isExpanded && <Button fullWidth mt="sm" component={Link} to="/accounts">Посмотреть все</Button>}
        </Card>
    );
};
