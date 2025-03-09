import { Stack, Input, Text, Title, Loader } from '@mantine/core';
import { Search } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';

import { ClientItem } from './components/ClientItem';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { Client } from '~/store/ClientsStore';
import { getClients } from '~/store/ClientsStore/ClientStore.action';
import { debounce } from '~/utils';

export const ClientsList = () => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { clients, isLoading } = useAppSelector((state) => state.clients);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredClients, setFilteredClients] = useState<Client[]>([]);

  useEffect(() => {
    if (accessToken) {
      dispatch(getClients(accessToken));
    }
  }, [accessToken]);

  useEffect(() => {
    if (clients) {
      setFilteredClients(clients);
    }
  }, [clients]);

  const handleInputChange = (value: string) => {
    setSearchQuery(value);
    debouncedSearch(value);
  };

  const debouncedSearch = useMemo(
    () =>
      debounce((value: string) => {
        const searchedClients = clients.filter((client) =>
          client.fullName.toLowerCase().includes(value.toLowerCase()),
        );
        setFilteredClients(searchedClients);
      }, 400),
    [clients],
  );

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
        value={searchQuery}
        onChange={(e) => handleInputChange(e.target.value)}
      />
      {isLoading ? (
        <Loader color="blue" />
      ) : (
        <>
          {filteredClients.length === 0 ? (
            <Text c="dimmed">Клиентов нет</Text>
          ) : (
            filteredClients.map((client) => (
              <ClientItem
                key={client.id}
                id={client.id}
                fullName={client.fullName}
                email={client.email}
                isBlocked={client.isBlocked}
              />
            ))
          )}
        </>
      )}
    </Stack>
  );
};
