import {
  CashOperation,
  CreditOperation,
  Account,
  UserAccountsData,
} from './store/AccountsStore/AccountsStore.types';
import { Client } from './store/ClientsStore/ClientsStore.types';

const cashOperationExample: CashOperation = {
  amountInRubles: 5000,
  time: new Date('2023-10-01T12:00:00Z'),
  operationType: 'deposit',
  operationCategory: 'salary',
};

const creditOperationExample: CreditOperation = {
  amountInRubles: -2000,
  time: new Date('2023-10-02T15:30:00Z'),
  operationType: 'withdrawal',
  operationCategory: 'groceries',
  creditId: 'credit-789',
};

const accountWithOperations: Account = {
  id: 'acc-456',
  name: 'Main Account',
  balance: 10000,
  operations: [cashOperationExample, creditOperationExample],
};

const accountWithoutOperations: Account = {
  id: 'acc-789',
  name: 'New Account',
  balance: 0,
  operations: [],
};

export const testUser: UserAccountsData = {
  userId: 'test-user',
  accounts: [accountWithOperations, accountWithoutOperations],
};

export const testClient: Client = {
  id: 'test-user',
  email: 'test@mail.com',
  fullName: 'Олег Олегов Олегович',
  isBlocked: false,
};
