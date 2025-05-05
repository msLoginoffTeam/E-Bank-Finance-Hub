import { useQuery } from '@tanstack/react-query';
import { queryKeys } from './queryKeys';
import { OperationsAPI } from '../services/operations.service';

export const useOperationsQuery = (accountId: string | null, limit?: number) => {
    return useQuery({
        queryKey: accountId ? [...queryKeys.accountOperations(accountId), { limit }] : [],
        queryFn: accountId ? () => OperationsAPI.getOperations(accountId) : async () => [],
        enabled: !!accountId,
        select: (data) => data ?? [],
        staleTime: 1000 * 60 * 5
    });
};

