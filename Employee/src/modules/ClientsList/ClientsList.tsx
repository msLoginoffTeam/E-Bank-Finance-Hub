import { Stack, Input, Title } from '@mantine/core';
import { Search } from 'lucide-react';
import { useEffect } from 'react';

import { ClientItem } from './components/ClientItem';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { getClients } from '~/store/ClientsStore/ClientStore.action';

export const ClientsList = () => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { clients } = useAppSelector((state) => state.clients);

  useEffect(() => {
    if (accessToken) {
      dispatch(getClients(accessToken));
    }
  }, [accessToken]);

  return (
    <Stack p="md" gap="xl" align="center" w="100%">
      <Title order={2}>Список клиентов</Title>
      <Input
        variant="filled"
        size="md"
        radius="xl"
        leftSectionPointerEvents="none"
        leftSection={<Search />}
        placeholder="Введите ФИО клиента"
        w="100%"
        maw={400}
        styles={{
          input: {
            background: '#fff',
          },
        }}
      />
      {clients.map((client) => (
        <ClientItem
          key={client.id}
          fullName={client.fullName}
          email={client.email}
          isBlocked={client.isBlocked}
        />
      ))}
    </Stack>
  );
};
