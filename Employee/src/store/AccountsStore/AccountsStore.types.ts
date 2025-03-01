export interface AccountState {
  accounts: UserAccountsData[];
  isLoading: boolean;
  error?: string;
}

export interface UserAccountsData {
  userId: string;
  accounts: Account[];
}

export interface Operation {
  amountInRubles: number;
  time: Date;
  operationType: string;
  operationCategory: string;
}

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CashOperation extends Operation {}

export interface CreditOperation extends Operation {
  creditId: string;
}

export interface Account {
  id: string;
  name: string;
  balance: number;
  operations: (CashOperation | CreditOperation)[];
}
