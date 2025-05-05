import {Card, Stack, Button, Title, ActionIcon, ScrollArea} from '@mantine/core';
import {AccountCard} from "../accounts/AccountCard.tsx";
import {Link} from "react-router-dom";
import {useHiddenAccounts} from "../../hooks/useHiddenAccounts.ts";
import {useState} from "react";

export const DashboardAccounts = ({ accounts, isExpanded }: { accounts: any[], isExpanded?: boolean }) => {

    const { hiddenIds, toggleHidden } = useHiddenAccounts();
    const [showHidden, setShowHidden] = useState(false);

    const visibleAccounts = hiddenIds ? accounts.filter(a => !hiddenIds.includes(a.id)) : accounts;
    const hiddenAccounts  = hiddenIds ? accounts.filter(a => hiddenIds.includes(a.id)) : [];

    const toDisplay = showHidden ? hiddenAccounts : visibleAccounts;

    return (
        <Card shadow="lg" p="md">
            <Title order={3} mb="sm" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                {showHidden ? 'Скрытые счета' : 'Мои счета'}
                <ActionIcon variant="light" size="lg">
                    {/*<IconPlus size={20} />*/}
                </ActionIcon>
            </Title>

            <Stack>
                <ScrollArea style={{ height: isExpanded ? "90vh" : undefined }}>
                {toDisplay && toDisplay.map((account) => (
                    <AccountCard
                        key={account.id}
                        account={account}
                        isHidden={hiddenIds.includes(account.id)}
                        onToggleHidden={() => toggleHidden(account.id)}/>
                ))}
                    <Button
                        fullWidth
                        mt="sm"
                        variant="outline"
                        onClick={() => setShowHidden(prev => !prev)}
                        disabled={!showHidden ? hiddenAccounts.length === 0 : visibleAccounts.length === 0}
                    >
                        {showHidden ? 'Показать видимые' : 'Показать скрытые'}
                    </Button>
            </ScrollArea>
            </Stack>

            {!isExpanded && <Button fullWidth mt="sm" component={Link} to="/accounts">Посмотреть все</Button>}
        </Card>
    );
};
