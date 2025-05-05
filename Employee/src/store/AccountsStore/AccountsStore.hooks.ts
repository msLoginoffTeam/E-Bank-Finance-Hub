import { useEffect } from 'react';

import { changeBalance, setOperation } from './AccountsStore.reducer';
import {
  Operation,
  OperationType,
  OperationCategory,
  BaseOperation,
  CreditOperation,
  CashOperation,
} from './AccountsStore.types';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { Payment, setPayment } from '~/store/CreditsStore';

export const useWebsocket = (id: string) => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);

  useEffect(() => {
    const ws = new WebSocket(
      `ws://localhost:5009/token=${accessToken}&accountId=${id}`,
    );

    ws.onmessage = (event) => {
      const message = JSON.parse(event.data);

      console.log(message);

      let operations: Operation[] = [];

      if (Array.isArray(message)) {
        operations = message.map(
          (op: {
            Amount: number;
            Time: string;
            OperationType: OperationType;
            OperationCategory: OperationCategory;
            CreditId?: string;
            IsSuccessful: boolean | null;
          }) => {
            const baseOperation: BaseOperation = {
              amount: op.Amount,
              time: op.Time,
              operationType: op.OperationType as OperationType,
              operationCategory: op.OperationCategory as OperationCategory,
            };

            if (baseOperation.operationCategory === OperationCategory.Credit) {
              return {
                ...baseOperation,
                creditId: op.CreditId,
                isSuccessful: op.IsSuccessful,
              } as CreditOperation;
            }

            return baseOperation as CashOperation;
          },
        );
      } else {
        const baseOperation: BaseOperation = {
          amount: message.Amount,
          time: message.Time,
          operationType: message.OperationType as OperationType,
          operationCategory: message.OperationCategory as OperationCategory,
        };

        if (baseOperation.operationType === OperationType.Income) {
          dispatch(
            changeBalance({ accountId: id, amount: baseOperation.amount }),
          );
        } else if (message.IsSuccessful !== false) {
          dispatch(
            changeBalance({ accountId: id, amount: -baseOperation.amount }),
          );
        }

        if (baseOperation.operationCategory === OperationCategory.Credit) {
          if (message.IsSuccessful !== false) {
            const payment: Payment = {
              id: message.Time,
              paymentAmount: message.Amount,
              paymentDate: message.Time,
              type: message.Type,
            };

            dispatch(
              setPayment({ creditId: message.CreditId, operation: payment }),
            );
          }

          operations.push({
            ...baseOperation,
            creditId: message.CreditId,
            isSuccessful: message.IsSuccessful,
          } as CreditOperation);
        } else {
          operations.push(baseOperation as CashOperation);
        }
      }

      dispatch(setOperation({ accountId: id, operations }));
    };

    return () => {
      ws.close();
    };
  }, [accessToken]);
};
