import { Card, Text, Button, Stack } from '@mantine/core';
import {CreditDetails} from "../../types/creditTypes.ts";
import {PayOffCreditModal} from "../../modals/PayOffCreditModal.tsx";
import {useState} from "react";

interface CreditCardProps {
    credit: CreditDetails;
    onPayOff?: () => void;
    hasOpenButton?: boolean
}

export const CreditCard = ({ credit, onPayOff, hasOpenButton }: CreditCardProps) => {
    const [isPayOffModalOpen, setIsPayOffModalOpen] = useState(false);
    console.log(credit)
    return (
        <div className="creditCard">
            <Card shadow="sm" padding="lg" radius="md" withBorder>
                <Stack>
                    <Text size="lg">
                        {credit.creditPlan.planName}
                    </Text>
                    <Text>Процентная ставка: {credit.creditPlan.planPercent}%</Text>
                    <Text>Сумма: {credit.amount} ₽</Text>
                    <Text>Осталось к выплате: {credit.remainingAmount} ₽</Text>
                    <Text>Дата закрытия: {new Date(credit.closingDate).toLocaleDateString()}</Text>
                    <Text>Статус: {credit.status}</Text>
                    {credit.status !== "Closed" && onPayOff && (
                        <Button color="red" onClick={() => setIsPayOffModalOpen(true)} fullWidth>
                            Погасить кредит
                        </Button>
                    )}
                    {hasOpenButton && (
                        <Button variant={'light'} onClick={onPayOff} fullWidth>
                            Подробнее
                        </Button>
                    )}
                </Stack>
            </Card>
            <PayOffCreditModal
                isOpen={isPayOffModalOpen}
                creditId={credit.id!}
                remainingAmount={credit!.remainingAmount}
                onClose={() => setIsPayOffModalOpen(false)}
            />
        </div>
    );
};
