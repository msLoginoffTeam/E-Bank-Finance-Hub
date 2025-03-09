import {useAccountsQuery} from "../queries/accounts.queries.ts";
import {useCreditsQuery} from "../queries/credits.queries.ts";
import {useOperationsQuery} from "../queries/operations.queries.ts";

export const useDashboardData = () => {
    const { data: accounts, isLoading: isLoadingAccounts } = useAccountsQuery();
    const { data: credits, isLoading: isLoadingCredits } = useCreditsQuery(); // Ограничиваем 3 кредитами

    const sortedAccounts = accounts ? [...accounts].sort((a, b) => Number(!b.isClosed) - Number(!a.isClosed)) : [];
    const limitedAccounts = sortedAccounts ? sortedAccounts.slice(0, 3) : [];

    const firstAccountId = limitedAccounts?.[0]?.id || null;
    const { data: operations, isLoading: isLoadingOperations } = useOperationsQuery(firstAccountId, 5);


    return {
        accounts: limitedAccounts,
        credits,
        operations,
        isLoading: isLoadingAccounts || isLoadingCredits || isLoadingOperations,
    };
};
