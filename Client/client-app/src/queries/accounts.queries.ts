import {useQuery, useMutation} from '@tanstack/react-query';
import { queryKeys } from './queryKeys';
import {AccountsAPI, AccountsStoreAPI, UserAPI} from '../services/accounts.service';
import { invalidateAccounts } from './invalidateQueries';

export const useAccountsQuery = (limit?: number) => {
    const query = useQuery({
        queryKey: queryKeys.accounts(),
        queryFn: AccountsStoreAPI.getAccounts, // API пока получает все счета
    });

    return {
        ...query,
        data: limit ? query.data?.slice(0, limit) : query.data,
    };
};

export const useOpenAccountMutation = (accountName: string, currency: string) => {
    return useMutation({
        mutationFn: () =>  AccountsStoreAPI.openAccount(accountName, currency),
        onSuccess: () => invalidateAccounts(),
    });
};

export const useCloseAccountMutation = (accountId: string) => {
    return useMutation({
        mutationFn: () => AccountsStoreAPI.closeAccount(accountId),
        onSuccess: () => invalidateAccounts(),
    });
};

export const useTransactionMutation = (accountId: string, type: 'deposit' | 'withdraw') => {

    return useMutation({
        mutationFn: async (amount: number) => {
                return AccountsAPI.deposit(accountId, amount, type === "deposit" ? "Income" : "Outcome");
        },
        onSuccess: () => invalidateAccounts()
    });
};

export const useUserQuery = () => {
    const query = useQuery({
        queryKey: queryKeys.userProfile(),
        queryFn: UserAPI.getUserId,
    });

    return {
        ...query,
        data: query.data,
    };
}

