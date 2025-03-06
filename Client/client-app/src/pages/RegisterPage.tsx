import { useState } from 'react';
import {TextInput, PasswordInput, Button, Title, Stack, Container, Center, Paper, Text} from '@mantine/core';
import {Link, useNavigate} from 'react-router-dom';
import {useAppDispatch} from "../hooks/redux.ts";
import {register} from "../store/AuthStore";

export const RegisterPage = () => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async () => {
        const result = await dispatch(register({ fullName: name, email, password }) as any);
        if (result.success) {
            navigate('/login');
        } else {
            setError(result.message);
        }
    };

    return (
        <Center style={{ height: '100vh' }}>
            <Container size="xs" style={{ minWidth: '420px' }}>
                <Paper shadow="md" p="xl" radius="md" withBorder>
                    <Stack>
                        <Title>Регистрация</Title>
                        <TextInput label="Имя" value={name} onChange={(e) => setName(e.target.value)} />
                        <TextInput label="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
                        <PasswordInput label="Пароль" value={password} onChange={(e) => setPassword(e.target.value)} />
                        {error && <Text color={'red'}>{error}</Text>}
                        <Button onClick={handleSubmit} fullWidth>
                            Зарегистрироваться
                        </Button>
                        <Text size="sm">
                            Уже есть аккаунт? <Link to="/login">Войти</Link>
                        </Text>
                    </Stack>
                </Paper>
            </Container>
        </Center>
    );
};
