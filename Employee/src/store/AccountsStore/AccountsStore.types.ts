export interface AccountState {
  accounts: Account[];
  isLoading: boolean;
  error?: string;
}

export interface BaseOperation {
  amount: number;
  time: string;
  operationType: OperationType;
  operationCategory: OperationCategory;
}

export enum OperationType {
  Income = 'Income',
  Outcome = 'Outcome',
}

export enum OperationCategory {
  Credit = 'Credit',
  Cash = 'Cash',
}

export enum Currency {
  Ruble = 'Ruble',
  Dollar = 'Dollar',
  Euro = 'Euro',
}

export interface CashOperation extends BaseOperation {
  operationCategory: OperationCategory.Cash;
}

export interface CreditOperation extends BaseOperation {
  operationCategory: OperationCategory.Credit;
  creditId: string;
  isSuccessful?: boolean | null;
}

export type Operation = CashOperation | CreditOperation;

export interface Account {
  id: string;
  name: string;
  balance: number;
  currency: Currency;
  isClosed: boolean;
  operations: Operation[];
  isLoadingOperations?: boolean;
}
