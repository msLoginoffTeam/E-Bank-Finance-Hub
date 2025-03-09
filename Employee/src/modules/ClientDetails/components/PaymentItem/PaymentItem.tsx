import { Card, Group, Stack, Text } from '@mantine/core';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';

import { Payment } from '~/store/CreditsStore';

export const PaymentItem = ({
  paymentAmount,
  paymentDate,
}: Omit<Payment, 'id' | 'type'>) => {
  return (
    <Card padding="md" radius="xl" withBorder>
      <Stack gap="xs">
        <Group>
          <Text fz="sm" c="dimmed">
            Сумма:
          </Text>
          <Text style={{ wordBreak: 'break-all' }}>{`${paymentAmount} ₽`}</Text>
        </Group>
        <Text fz="sm" c="dimmed">
          {format(new Date(paymentDate), 'dd MMMM yyyy, HH:mm', {
            locale: ru,
          })}
        </Text>
      </Stack>
    </Card>
  );
};
