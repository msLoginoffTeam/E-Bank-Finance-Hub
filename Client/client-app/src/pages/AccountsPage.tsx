import {useAccountsQuery} from '../queries/accounts.queries';
//import { Card, Center, Container, Loader, Stack, Text, Title } from '@mantine/core';
import {DashboardAccounts} from "../components/dashboard/DashboardAccounts.tsx";

export const AccountsPage = () => {
    const {data: accounts} = useAccountsQuery();
    //const { mutate: openAccount, isPending: isOpening } = useOpenAccountMutation();

    return (
        <DashboardAccounts accounts={accounts.sort((a: any, b: any) => Number(!b.isClosed) - Number(!a.isClosed))}
                           isExpanded={true}>
        </DashboardAccounts>
    );
};
