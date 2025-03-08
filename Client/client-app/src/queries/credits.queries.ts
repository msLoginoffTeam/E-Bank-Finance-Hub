import { useQuery } from '@tanstack/react-query';
import { queryKeys } from './queryKeys';
import { CreditsAPI } from '../services/credits.service';

export const useCreditsQuery = (limit?: number) => {
    const query = useQuery({
        queryKey: limit ? [...queryKeys.credits(), { limit }] : queryKeys.credits(),
        queryFn: CreditsAPI.getActiveCredits,
    });

    return {
        ...query,
        data: limit ? query.data?.slice(0, limit) : query.data,
    };
};
