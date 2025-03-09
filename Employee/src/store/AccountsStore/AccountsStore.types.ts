export interface AccountState {
  accounts: Account[];
  isLoading: boolean;
  error?: string;
}

export interface BaseOperation {
  amountInRubles: number;
  time: Date;
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

export interface CashOperation extends BaseOperation {
  operationCategory: OperationCategory.Cash;
}

export interface CreditOperation extends BaseOperation {
  operationCategory: OperationCategory.Credit;
  creditId: string;
}

export type Operation = CashOperation | CreditOperation;

export interface Account {
  id: string;
  name: string;
  balanceInRubles: number;
  isClosed: boolean;
  operations: Operation[];
  isLoadingOperations?: boolean;
}
