import { useQuery, useMutation } from '@tanstack/react-query';
import { queryKeys } from './queryKeys';
import { AccountsStoreAPI } from '../services/accounts.service';
import { invalidateAccounts } from './invalidateQueries';

// Получаем список всех счетов
export const useAccountsQuery = (limit?: number) => {
    const query = useQuery({
        queryKey: queryKeys.accounts(),
        queryFn: AccountsStoreAPI.getAccounts, // API пока получает все счета
    });

    // Если API не поддерживает limit, фильтруем на фронте
    return {
        ...query,
        data: limit ? query.data?.slice(0, limit) : query.data,
    };
};

// Открываем новый счёт
export const useOpenAccountMutation = () => {
    return useMutation({
        mutationFn: AccountsStoreAPI.openAccount,
        onSuccess: () => invalidateAccounts(),
    });
};

// Закрываем счёт
export const useCloseAccountMutation = () => {
    return useMutation({
        mutationFn: AccountsStoreAPI.closeAccount,
        onSuccess: () => invalidateAccounts(),
    });
};
