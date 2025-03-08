import { Card, Text, Button } from '@mantine/core';

export const AccountCard = ({ account, onClose }: { account: any, onClose?: () => void }) => {
    return (
        <Card shadow="md" p="md" radius="md">
            <Text>Номер счёта: {account.id}</Text>
            <Text>Баланс: {account.balance} ₽</Text>
            <Text>Дата открытия: {new Date(account.createdAt).toLocaleDateString()}</Text>

            {onClose && (
                <Button color="red" fullWidth mt="md" onClick={onClose}>
                    Закрыть счёт
                </Button>
            )}
        </Card>
    );
};
