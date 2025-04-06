import {
  ActionIcon,
  Button,
  Card,
  Collapse,
  Group,
  Loader,
  ScrollArea,
  Stack,
  Text,
  useMantineColorScheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { ChevronDown, ChevronUp, Eye, EyeOff } from 'lucide-react';
import { useEffect } from 'react';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { OperationItem } from '~/modules/ClientDetails/components/OperationItem';
import { Account, Currency, useWebsocket } from '~/store/AccountsStore';
import { OperationCategory } from '~/store/AccountsStore/AccountsStore.types';
import { editSettings } from '~/store/AppStore';
import { getCreditRating } from '~/store/CreditsStore';

export const AccountItem = ({
  id,
  name,
  balance,
  currency,
  isClosed,
  operations,
  isLoadingOperations,
  isHidden,
}: Account & { isHidden: boolean }) => {
  const dispatch = useAppDispatch();
  const { profile, accessToken } = useAppSelector((state) => state.auth);
  const [opened, { toggle }] = useDisclosure(false);
  const { colorScheme } = useMantineColorScheme();
  const { hiddenAccounts } = useAppSelector((state) => state.app);
  const { client } = useAppSelector((state) => state.clients);
  useWebsocket(id);

  const handleToggle = () => {
    toggle();
  };

  const getCurrencySymbol = () => {
    switch (currency) {
      case Currency.Dollar:
        return '$';
      case Currency.Euro:
        return '€';
      case Currency.Ruble:
        return '₽';
    }
  };

  const handleHideAccount = () => {
    let newHiddenAccounts: string[] = [];

    if (isHidden) {
      newHiddenAccounts = hiddenAccounts.filter((hiddenId) => hiddenId !== id);
    } else {
      newHiddenAccounts = [...hiddenAccounts, id];
    }

    dispatch(
      editSettings({
        id: profile.id,
        newSettings: { hiddenAccounts: newHiddenAccounts },
      }),
    );
  };

  useEffect(() => {
    if (accessToken && client.id) {
      dispatch(getCreditRating({ accessToken, clientId: client.id }));
    }
  }, [accessToken]);

  /*useEffect(() => {
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
        } else {
          dispatch(
            changeBalance({ accountId: id, amount: -baseOperation.amount }),
          );
        }

        if (baseOperation.operationCategory === OperationCategory.Credit) {
          const payment: Payment = {
            id: message.Time,
            paymentAmount: message.Amount,
            paymentDate: message.Time,
            type: message.Type,
          };

          dispatch(
            setPayment({ creditId: message.CreditId, operation: payment }),
          );

          operations.push({
            ...baseOperation,
            creditId: message.CreditId,
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
  }, []);*/

  return (
    <Card padding="md" radius="xl" shadow="md" withBorder>
      <Stack gap="md">
        <Group justify="space-between">
          <Group>
            <Text c="dimmed">Название счета:</Text>
            <Text>{name}</Text>
          </Group>
          <ActionIcon
            variant="transparent"
            c={colorScheme === 'dark' ? 'gray' : 'dark'}
            onClick={handleHideAccount}
          >
            {isHidden ? <Eye /> : <EyeOff />}
          </ActionIcon>
        </Group>
        <Group>
          <Text c="dimmed">Баланс:</Text>
          <Text>
            {balance} {getCurrencySymbol()}
          </Text>
        </Group>
        <Group>
          <Text c="dimmed">Статус счёта:</Text>
          <Text>{isClosed ? 'Закрыт' : 'Активен'}</Text>
        </Group>
        <Button
          variant="light"
          radius="xl"
          rightSection={opened ? <ChevronUp /> : <ChevronDown />}
          onClick={handleToggle}
        >
          Раскрыть операции
        </Button>
        <Collapse in={opened}>
          <Stack gap="xs">
            {!isLoadingOperations ? (
              <Loader color="blue" />
            ) : (
              <>
                {operations.length === 0 ? (
                  <Text c="dimmed">Нет операций</Text>
                ) : (
                  <ScrollArea.Autosize mah={300} p="md">
                    <Stack gap="xs">
                      {operations.map((op, index) => {
                        if (op.operationCategory === OperationCategory.Credit) {
                          return (
                            <OperationItem
                              // eslint-disable-next-line react/no-array-index-key
                              key={index}
                              operationType={op.operationType}
                              operationCategory={op.operationCategory}
                              amount={op.amount}
                              time={op.time}
                              isSuccessful={op.isSuccessful}
                            />
                          );
                        } else {
                          return (
                            <OperationItem
                              // eslint-disable-next-line react/no-array-index-key
                              key={index}
                              operationType={op.operationType}
                              operationCategory={op.operationCategory}
                              amount={op.amount}
                              time={op.time}
                            />
                          );
                        }
                      })}
                    </Stack>
                  </ScrollArea.Autosize>
                )}
              </>
            )}
          </Stack>
        </Collapse>
      </Stack>
    </Card>
  );
};
