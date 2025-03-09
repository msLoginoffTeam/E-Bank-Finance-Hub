export type CreditPlan = {
    id: string;
    planName: string;
    planPercent: number;
    status: number; // 0 - Open, 1 - Closed
};

export type PaymentHistoryEntry = {
    date: string;
    type: string;
    amount: number;
};

export type CreditDetails = {
    id: string;
    creditPlan: CreditPlan;
    amount: number;
    closingDate: string;
    remainingAmount: number;
    status: string; // 0 - Open, 1 - Closed
    paymentHistory: PaymentHistoryEntry[] | undefined;
};
