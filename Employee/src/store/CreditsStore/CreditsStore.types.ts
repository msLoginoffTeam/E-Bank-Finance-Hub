export interface CreditsState {
  planList: CreditPlan[];
  credits: ClientCreditForEmployeeResponse[];
  isLoading: boolean;
  error?: string;
}

export interface GetCredidtsPlansResponse {
  planList: CreditPlan[];
}

export interface CreditPlan {
  id: string;
  planName: string;
  planPercent: number;
  status: CreditStatus;
}

export enum CreditStatus {
  Open,
  Archive,
}

export enum ClientCreditStatus {
  Open,
  Closed,
  DoublePercentage,
  Expired,
}

export interface ClientCreditForEmployeeResponse {
  id: string;
  clientId: string;
  accountId: string;
  creditPlan: CreditPlan;
  amount: number;
  closingDate: Date;
  remainingAmount: number;
  status: ClientCreditStatus;
  paymentHistory: Payment[];
  isLoadingHistory?: boolean;
}

export interface Pagination {
  requestedNumber: number;
  pageNumber: number;
  actualNumber: number;
  fullCount: number;
}

export interface Payment {
  id: string;
  paymentAmount: number;
  paymentDate: Date;
  type: PaymentType;
}

export enum PaymentType {
  Automatic,
  ByClient,
}
