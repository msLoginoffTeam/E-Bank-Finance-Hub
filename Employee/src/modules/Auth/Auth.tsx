import { Button, Card, Group, Stack, TextInput, Title } from '@mantine/core';
import { hasLength, isEmail, useForm } from '@mantine/form';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { useNotification } from '~/hooks/useNotification';
import { AuthCredentials, loginEmployee } from '~/store/AuthStore';

export const Auth = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const error = useAppSelector((state) => state.auth.error);
  const { showSuccess, showError } = useNotification();

  const form = useForm<AuthCredentials>({
    mode: 'uncontrolled',
    initialValues: {
      email: '',
      password: '',
    },

    validate: {
      password: hasLength({ min: 6 }, 'Пароль должен быть длинее 6 символов'),
      email: isEmail('Неправильный формат Email'),
    },
  });

  const handleSubmit = async () => {
    const values = form.getValues();
    const result = await dispatch(loginEmployee({ ...values }));

    if (result.meta.requestStatus === 'fulfilled') {
      showSuccess('Вы успешно авторизовались');
      navigate('/');
    }
  };

  useEffect(() => {
    if (error) showError(error);
  }, [error]);

  return (
    <form
      onSubmit={form.onSubmit(() => {
        handleSubmit();
      })}
      style={{ width: '100%' }}
    >
      <Stack gap="md" align="center" w="100%">
        <Title>Вход в аккаунт</Title>
        <Card padding="lg" radius="xl" w="100%" maw={600}>
          <Stack gap="md">
            <TextInput
              withAsterisk
              radius="md"
              label="Email"
              placeholder="Введите email"
              key={form.key('email')}
              {...form.getInputProps('email')}
            />
            <TextInput
              withAsterisk
              radius="md"
              label="Пароль"
              placeholder="Введите пароль"
              key={form.key('password')}
              {...form.getInputProps('password')}
            />
            <Group justify="center">
              <Button
                type="submit"
                variant="light"
                radius="xl"
                w="100%"
                maw={200}
              >
                Войти
              </Button>
            </Group>
          </Stack>
        </Card>
      </Stack>
    </form>
  );
};
