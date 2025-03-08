export const queryKeys = {
    accounts: () => ['accounts'],
    account: (accountId: string) => ['account', accountId],
    accountOperations: (accountId: string) => ['accountOperations', accountId],

    credits: () => ['credits'],
    credit: (creditId: string) => ['credit', creditId],

    userProfile: () => ['userProfile'],
};