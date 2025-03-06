import { Container, Center, Stack, Title, TextInput, PasswordInput, Button, Paper, Text } from '@mantine/core';
import { useForm } from '@mantine/form';
import { useNavigate, Link } from 'react-router-dom';
import {useEffect, useState} from 'react';
import {useAppDispatch, useAppSelector} from "../hooks/redux.ts";
import {clearAuthError, login} from "../store/AuthStore";

export const LoginPage = () => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const token = useAppSelector((state) => state.auth.token);
    const backendError = useAppSelector((state) => state.auth.error);
    const [loading, setLoading] = useState(false);

    const form = useForm({
        initialValues: {
            email: '',
            password: '',
        },
        validate: {
            email: (value) =>
                /^\S+@\S+\.\S+$/.test(value) ? null : 'Введите корректный email',
            password: (value) =>
                value.length >= 6 ? null : 'Пароль должен быть минимум 6 символов',
        },
    });

    const handleSubmit = async (values: { email: string; password: string }) => {
        setLoading(true);
        dispatch(clearAuthError());
        await dispatch(login(values));
        setLoading(false);
    };

    useEffect(() => {
        if (token) {
            navigate('/');
        }
    }, [token]);

    return (
        <Center style={{ height: '100vh' }}>
            <Container size="xs">
                <Paper shadow="md" p="xl" radius="md" withBorder>
                    <form onSubmit={form.onSubmit(handleSubmit)}>
                        <Stack>
                            <Title>Вход</Title>

                            <TextInput
                                label="Email"
                                placeholder="Введите email"
                                {...form.getInputProps('email')}
                            />

                            <PasswordInput
                                label="Пароль"
                                placeholder="Введите пароль"
                                {...form.getInputProps('password')}
                            />

                            <Button type="submit" loading={loading} fullWidth>
                                Войти
                            </Button>

                            {backendError && <Text color="red">{backendError}</Text>}

                            <Text size="sm">
                                Еще не зарегистрированы? <Link to="/register">Зарегистрироваться</Link>
                            </Text>
                        </Stack>
                    </form>
                </Paper>
            </Container>
        </Center>
    );
};
