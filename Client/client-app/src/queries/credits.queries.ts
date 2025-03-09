import { useQuery, useMutation } from '@tanstack/react-query';
import { queryKeys } from './queryKeys';
import {CreditAPI} from "../services/credits.service.ts";

// Получение всех кредитов пользователя
export const useCreditsQuery = () => {
    return useQuery({
        queryKey: queryKeys.credits(),
        queryFn: CreditAPI.getCredits,
    });
};

// Получение деталей конкретного кредита (с историей)
export const useCreditDetailsQuery = (creditId: string) => {
    return useQuery({
        queryKey: queryKeys.creditDetails(creditId),
        queryFn: () => CreditAPI.getCreditDetails(creditId),
        enabled: !!creditId,
    });
};

// Получение доступных тарифов (только активных)
export const useCreditPlansQuery = () => {
    return useQuery({
        queryKey: queryKeys.creditPlans(),
        queryFn: CreditAPI.getCreditPlans,
        //select: (plans: any) => plans.filter((plan: any) => plan.status === 0), // 0 == Open
    });
};

// Оформление нового кредита
export const useCreateCreditMutation = () => {
    return useMutation({
        mutationFn: CreditAPI.createCredit,
    });
};

// Погашение кредита
export const usePayOffCreditMutation = () => {
    return useMutation({
        mutationFn: CreditAPI.payOffCredit,
    });
};
