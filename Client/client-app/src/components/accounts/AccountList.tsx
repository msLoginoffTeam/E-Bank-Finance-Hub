import { Stack } from '@mantine/core';
import { AccountCard } from './AccountCard';

export const AccountList = ({ accounts }: { accounts: any[], onCloseAccount?: (id: string) => void }) => {
    return (
        <Stack>
            {accounts.map(account => (
                <AccountCard key={account.id} account={account} />
            ))}
        </Stack>
    );
};
