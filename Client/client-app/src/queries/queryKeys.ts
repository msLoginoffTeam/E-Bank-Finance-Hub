export const queryKeys = {
    accounts: () => ['accounts'],
    account: (accountId: string) => ['account', accountId],
    accountOperations: (accountId: string) => ['accountOperations', accountId],

    credits: () => ['credits'],
    creditDetails: (creditId: string) => ['creditDetails', creditId],
    creditOperations: (creditId: string) => ['creditOperations', creditId], // История платежей по кредиту
    creditPlans: () => ['creditPlans'], // Доступные кредитные тарифы

    userProfile: () => ['userProfile'],

    allOperations: () => ['allOperations'],

    theme: (userId: string) => ["theme", userId],
};