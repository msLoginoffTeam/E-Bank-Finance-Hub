import { Card, Text, Button, Stack } from '@mantine/core';
import {CreditDetails} from "../../types/creditTypes.ts";

interface CreditCardProps {
    credit: CreditDetails;
    onPayOff?: () => void;
}

export const CreditCard = ({ credit, onPayOff }: CreditCardProps) => {
    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Stack>
                <Text size="lg">
                    {credit.creditPlan.planName}
                </Text>
                <Text>Процентная ставка: {credit.creditPlan.planPercent}%</Text>
                <Text>Сумма: {credit.amount} ₽</Text>
                <Text>Дата закрытия: {new Date(credit.closingDate).toLocaleDateString()}</Text>
                <Text>Статус: {credit.status}</Text>

                {credit.status !== "Closed" && onPayOff && (
                    <Button color="red" onClick={onPayOff} fullWidth>
                        Погасить кредит
                    </Button>
                )}
            </Stack>
        </Card>
    );
};
