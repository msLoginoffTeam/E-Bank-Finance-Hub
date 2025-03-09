import { Modal, Button, TextInput, Text, Group } from '@mantine/core';
import { useState } from 'react';
import {useTransactionMutation} from "../queries/accounts.queries.ts";

export const ModalTransaction = ({ opened, onClose, account, transactionType }: {
    opened: boolean;
    onClose: () => void;
    account: any;
    transactionType: 'deposit' | 'withdraw';
}) => {
    const [amount, setAmount] = useState('');
    const [error, setError] = useState<string | null>(null);
    const mutation = useTransactionMutation(account.id, transactionType);

    const handleSubmit = async () => {
        setError(null);
        const value = Number(amount);

        if (isNaN(value) || value <= 0) {
            setError('Введите корректную сумму');
            return;
        }

        if (transactionType === 'withdraw' && value > account.balance) {
            setError('Недостаточно средств');
            return;
        }

        try {
            await mutation.mutateAsync(value);
            onClose();
        } catch (err: any) {
            console.log(err)
            setError(err.response.data.message);
        }
    };

    return (
        <Modal opened={opened} onClose={onClose} title={transactionType === 'deposit' ? 'Пополнение счета' : 'Снятие средств'}>
            <Text>Введите сумму:</Text>
            <TextInput value={amount} onChange={(e) => {
                setAmount(e.currentTarget.value);
                setError(null);
            } } placeholder="Введите сумму" />

            {error && <Text color="red">{error}</Text>}

            <Group mt="md">
                <Button loading={mutation.isPending} fullWidth color={transactionType === 'deposit' ? 'green' : 'red'} onClick={handleSubmit}>
                    {transactionType === 'deposit' ? 'Пополнить' : 'Снять'}
                </Button>
            </Group>
        </Modal>
    );
};
