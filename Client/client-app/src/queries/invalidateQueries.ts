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

export const invalidateCreditDetails = async (creditId: string) => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.creditDetails(creditId) });
};

export const invalidateCreditOperations = async (creditId: string) => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.creditOperations(creditId) });
};

// Инвалидация тарифов кредитов
export const invalidateCreditPlans = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.creditPlans() });
};

export const invalidateUserProfile = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.userProfile() });
};

export const invalidateTheme = async (userId: string) => {
    await queryClient.invalidateQueries({queryKey: queryKeys.theme(userId)});
};
