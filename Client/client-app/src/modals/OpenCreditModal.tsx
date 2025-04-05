import { Modal, Button, Select, NumberInput, Stack } from '@mantine/core';
import { useState } from 'react';
import { useAccountsQuery } from '../queries/accounts.queries';
import { useCreateCreditMutation, useCreditPlansQuery } from '../queries/credits.queries';

interface OpenCreditModalProps {
    onClose: () => void;
}

export const OpenCreditModal = ({ onClose }: OpenCreditModalProps) => {
    const { data: plans } = useCreditPlansQuery();
    const { data: accounts } = useAccountsQuery();
    const { mutate: openCredit, isPending } = useCreateCreditMutation();

    const [planId, setPlanId] = useState<string | null>(null);
    const [accountId, setAccountId] = useState<string | null>(null);
    const [amount, setAmount] = useState<number>(1000);
    const [closingDate, setClosingDate] = useState<Date | null>(new Date());

    const handleSubmit = () => {
        if (!planId || !accountId || !closingDate) return;

        openCredit({
            creditPlanId: planId,
            accountId,
            amount,
            closingDate: closingDate.toISOString()
        }, {
            onSuccess: onClose
        });
    };

    return (
        <Modal opened onClose={onClose} title="Оформление кредита">
            <Stack>
                <Select
                    label="Выберите тариф"
                    placeholder="Выберите тариф"
                    data={plans?.filter((p: any) => !p.status)
                        .map((p: any) => ({ value: p.id, label: `${p.planName} (${p.planPercent}%)` }))}
                    value={planId}
                    onChange={setPlanId}
                    disabled={isPending}
                />
                <Select
                    label="Выберите счёт"
                    placeholder="Выберите счёт"
                    data={accounts?.filter((a: any) => a.status !== 'Closed')
                        .map((a: any) => ({ value: a.id, label: `Счет ${a.name}` }))}
                    value={accountId}
                    onChange={setAccountId}
                    disabled={isPending}
                />
                <NumberInput
                    label="Сумма"
                    min={1000}
                    value={amount}
                    onChange={(value) => setAmount(Number(value) || 1000)}
                    disabled={isPending}
                />
                <input
                    type="date"
                    value={closingDate?.toISOString().split('T')[0]}
                    onChange={(e) => setClosingDate(new Date(e.target.value))}
                />
                <Button onClick={handleSubmit} disabled={isPending || !planId || !accountId || !closingDate}>
                    {isPending ? 'Оформляем...' : 'Оформить'}
                </Button>
            </Stack>
        </Modal>
    );
};
