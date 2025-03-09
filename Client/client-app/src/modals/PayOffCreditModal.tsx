import { Modal, Button, Select, NumberInput, Stack } from '@mantine/core';
import { useState } from 'react';
import { useAccountsQuery } from '../queries/accounts.queries';
import { usePayOffCreditMutation } from '../queries/credits.queries';

interface PayOffCreditModalProps {
    creditId: string;
    remainingAmount: number;
    onClose: () => void;
    isOpen: boolean;
}

export const PayOffCreditModal = ({ creditId, remainingAmount, onClose, isOpen }: PayOffCreditModalProps) => {
    const { data: accounts } = useAccountsQuery();
    const { mutate: payOffCredit, isPending } = usePayOffCreditMutation();

    const [accountId, setAccountId] = useState<string | null>(null);
    const [amount, setAmount] = useState<number>(remainingAmount);

    const handleSubmit = () => {
        if (!accountId || amount <= 0) return;

        payOffCredit(
            { accountId, creditId, amount },
            { onSuccess: onClose }
        );
    };

    return (
        <Modal opened={isOpen} onClose={onClose} title="Погашение кредита">
            <Stack>
                <Select
                    label="Выберите счёт для списания"
                    placeholder="Выберите счёт"
                    data={accounts?.filter((a: any) => a.status !== 'Closed')
                        .map((a: any) => ({ value: a.id, label: `Счет ${a.name}` }))}
                    value={accountId}
                    onChange={setAccountId}
                    disabled={isPending}
                />
                <NumberInput
                    label="Сумма погашения"
                    min={1}
                    max={remainingAmount}
                    value={amount}
                    onChange={(value) => setAmount(Number(value) || 0)}
                    disabled={isPending}
                />
                <Button onClick={handleSubmit} disabled={isPending || !accountId || amount <= 0}>
                    {isPending ? 'Погашаем...' : 'Погасить кредит'}
                </Button>
            </Stack>
        </Modal>
    );
};
