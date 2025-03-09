import {
  Avatar,
  Badge,
  Card,
  Group,
  Loader,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { useEffect } from 'react';
import { useParams } from 'react-router-dom';

import { AccountItem } from './components/AccountItem';
import { CreditItem } from './components/CreditItem';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { getClientAccounts } from '~/store/AccountsStore';
import { getClient } from '~/store/ClientsStore';
import { getClientCredits } from '~/store/CreditsStore';

export const ClientDetails = () => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { client, isLoading } = useAppSelector((state) => state.clients);
  const { accounts } = useAppSelector((state) => state.accounts);
  const accountsLoading = useAppSelector((state) => state.accounts.isLoading);
  const { credits } = useAppSelector((state) => state.credits);
  const creditsLoading = useAppSelector((state) => state.credits.isLoading);

  const { id } = useParams();

  useEffect(() => {
    if (accessToken && id) {
      dispatch(getClient({ accessToken, id }));
      dispatch(getClientAccounts({ accessToken, id }));
      dispatch(getClientCredits({ accessToken, id }));
    }
  }, [id, accessToken]);

  if (isLoading) {
    return <Loader color="blue" size="xl" />;
  }

  return (
    <>
      <Title order={2}>Детальная информация клиента</Title>
      <Card mt="xl" padding="lg" radius="xl" w="100%" maw={600}>
        <Stack gap="md">
          <Group w="100%" justify="center">
            <Avatar size="xl" name={client.fullName} color="initials" />
          </Group>
          <Group>
            <Text c="dimmed">ФИО:</Text>
            <Text style={{ wordBreak: 'break-all' }}>{client.fullName}</Text>
          </Group>
          <Group>
            <Text c="dimmed">Email:</Text>
            <Text>{client.email}</Text>
          </Group>
          {client.isBlocked && <Badge color="red">Заблокирован</Badge>}
          <Title order={4}>Счета:</Title>
          {accountsLoading ? (
            <Loader color="blue" />
          ) : (
            <>
              {accounts.length === 0 ? (
                <Text c="dimmed">Нет счетов</Text>
              ) : (
                accounts.map((acc) => (
                  <AccountItem
                    key={acc.id}
                    id={acc.id}
                    name={acc.name}
                    balanceInRubles={acc.balanceInRubles}
                    isClosed={acc.isClosed}
                    operations={acc.operations}
                    isLoadingOperations={acc.isLoadingOperations}
                  />
                ))
              )}
            </>
          )}
          <Title order={4}>Кредиты:</Title>
          {creditsLoading ? (
            <Loader color="blue" />
          ) : (
            <>
              {credits.length === 0 ? (
                <Text c="dimmed">Нет кредитов</Text>
              ) : (
                credits.map((credit) => (
                  <CreditItem
                    key={credit.id}
                    id={credit.id}
                    creditPlan={credit.creditPlan}
                    amount={credit.amount}
                    closingDate={credit.closingDate}
                    remainingAmount={credit.remainingAmount}
                    status={credit.status}
                    paymentHistory={credit.paymentHistory}
                    isLoadingHistory={credit.isLoadingHistory}
                  />
                ))
              )}
            </>
          )}
        </Stack>
      </Card>
    </>
  );
};
