import { queryClient } from '../lib/reactQueryClient';
import { queryKeys } from './queryKeys';

export const invalidateAccounts = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.accounts() });
};

export const invalidateAccount = async (accountId: string) => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.account(accountId) });
};

export const invalidateAccountOperations = async (accountId: string) => {
    await queryClient.invalidateQueries({
        queryKey: queryKeys.accountOperations(accountId),
    });
};

export const invalidateCredits = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.credits() });
};

export const invalidateCredit = async (creditId: string) => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.credit(creditId) });
};

export const invalidateUserProfile = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.userProfile() });
};
