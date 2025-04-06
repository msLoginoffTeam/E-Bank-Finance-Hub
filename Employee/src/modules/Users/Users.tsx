import {
  Button,
  Card,
  Flex,
  Group,
  Loader,
  SegmentedControl,
  Stack,
  Tabs,
  TextInput,
  Title,
} from '@mantine/core';
import { hasLength, isEmail, useForm } from '@mantine/form';
import { useEffect, useState } from 'react';

import { UserItem } from './components/UserItem';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { useNotification } from '~/hooks/useNotification';
import { createUser, CreateUser, UserRole } from '~/store/AuthStore';
import { getClients, getEmployees } from '~/store/ClientsStore';
import { decodeJwt } from '~/utils';

export const Users = () => {
  const { accessToken, error } = useAppSelector((state) => state.auth);
  const { clients, employees, isLoading } = useAppSelector(
    (state) => state.clients,
  );
  const { showSuccess, showError } = useNotification();
  const dispatch = useAppDispatch();
  const [activeTab, setActiveTab] = useState<string | null>('create');
  const [role, setRole] = useState('');

  const form = useForm<CreateUser>({
    mode: 'uncontrolled',
    initialValues: {
      email: '',
      password: '',
      fullName: '',
      userRole: UserRole.Client,
    },

    validate: {
      password: hasLength({ min: 6 }, 'Пароль должен быть длинее 6 символов'),
      email: isEmail('Неправильный формат Email'),
      fullName: hasLength({ min: 6 }, 'Имя должно быть длинее 6 символов'),
    },
  });

  const handleSubmit = async () => {
    const values = form.getValues();
    console.log(values);

    if (accessToken) {
      const result = await dispatch(
        createUser({
          accessToken,
          userData: {
            email: values.email,
            password: values.password,
            fullName: values.fullName,
            userRole: values.userRole,
          },
        }),
      );

      if (result.meta.requestStatus === 'fulfilled') {
        showSuccess('Пользователь создан');
      }
    }
  };

  useEffect(() => {
    if (accessToken) {
      dispatch(getClients(accessToken));
    }
  }, [accessToken]);

  useEffect(() => {
    if (accessToken && role === 'Manager') {
      dispatch(getEmployees(accessToken));
    }
  }, [accessToken, role]);

  useEffect(() => {
    if (accessToken) {
      const role =
        decodeJwt(accessToken)[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ];
      setRole(role);
    }
  }, [accessToken]);

  useEffect(() => {
    if (error) showError(error);
  }, [error]);

  return (
    <Card mt="xl" padding="lg" radius="xl" w="100%" maw={600}>
      <Flex justify="center" align="center">
        <Tabs
          value={activeTab}
          onChange={setActiveTab}
          defaultValue="create"
          w="100%"
        >
          <Tabs.List grow justify="center">
            <Tabs.Tab value="create">Создание</Tabs.Tab>
            <Tabs.Tab value="block">Блокировка</Tabs.Tab>
          </Tabs.List>
          <Tabs.Panel value="create" p="md">
            <form
              onSubmit={form.onSubmit(() => {
                handleSubmit();
              })}
              style={{ width: '100%' }}
            >
              <Stack gap="xs" align="center">
                <Title order={4}>Создание пользователя</Title>
                <SegmentedControl
                  key={form.key('userRole')}
                  {...form.getInputProps('userRole')}
                  data={[
                    { label: 'Клиент', value: UserRole.Client },
                    { label: 'Сотрудник', value: UserRole.Employee },
                  ]}
                />
                <TextInput
                  w="100%"
                  withAsterisk
                  radius="md"
                  label="Email"
                  placeholder="Введите email"
                  key={form.key('email')}
                  {...form.getInputProps('email')}
                />
                <TextInput
                  w="100%"
                  withAsterisk
                  radius="md"
                  label="Пароль"
                  placeholder="Введите пароль"
                  key={form.key('password')}
                  {...form.getInputProps('password')}
                />
                <TextInput
                  w="100%"
                  withAsterisk
                  radius="md"
                  label="ФИО"
                  placeholder="Введите ФИО"
                  key={form.key('fullName')}
                  {...form.getInputProps('fullName')}
                />
                <Group justify="center">
                  <Button
                    type="submit"
                    variant="light"
                    radius="xl"
                    w="100%"
                    maw={200}
                  >
                    Создать
                  </Button>
                </Group>
              </Stack>
            </form>
          </Tabs.Panel>
          <Tabs.Panel value="block" p="md">
            <Stack gap="xs" align="center">
              <Title order={4}>Блокировка клиента</Title>
              {isLoading ? (
                <Loader color="blue" />
              ) : (
                <>
                  {clients.map((client) => (
                    <UserItem
                      key={client.id}
                      id={client.id}
                      email={client.email}
                      fullName={client.fullName}
                      isBlocked={client.isBlocked}
                    />
                  ))}
                </>
              )}
              {role === 'Manager' && (
                <Title order={4}>Блокировка сотрудника</Title>
              )}
              {isLoading ? (
                <Loader color="blue" />
              ) : (
                <>
                  {employees.map((employee) => (
                    <UserItem
                      key={employee.id}
                      id={employee.id}
                      email={employee.email}
                      fullName={employee.fullName}
                      isBlocked={employee.isBlocked}
                      role={role}
                    />
                  ))}
                </>
              )}
            </Stack>
          </Tabs.Panel>
        </Tabs>
      </Flex>
    </Card>
  );
};
