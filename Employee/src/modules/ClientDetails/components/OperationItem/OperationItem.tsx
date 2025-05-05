import { Badge, Card, Group, Stack, Text } from '@mantine/core';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';

import { Operation } from '~/store/AccountsStore';

export const OperationItem = ({
  operationType,
  amount,
  operationCategory,
  time,
  isSuccessful,
}: Omit<Operation, 'creditId'> & { isSuccessful?: boolean | null }) => {
  return (
    <Card padding="md" radius="xl" withBorder>
      <Stack gap="xs">
        <Group justify="space-between">
          <Group>
            <Text fz="sm" c="dimmed">
              {operationType === 'Outcome' ? 'Списание:' : 'Пополнение:'}
            </Text>
            <Text
              c={amount < 0 || operationType === 'Outcome' ? 'red' : 'teal'}
              style={{ wordBreak: 'break-all' }}
            >
              {`${amount} ₽`}
            </Text>
          </Group>
          {isSuccessful === false && (
            <Badge color="red" radius="sm">
              Просрочено
            </Badge>
          )}
        </Group>
        <Text fz="sm" c="dimmed">
          {format(new Date(time), 'dd MMMM yyyy, HH:mm', {
            locale: ru,
          })}
        </Text>
        <Badge color="blue" radius="sm">
          {operationCategory}
        </Badge>
      </Stack>
    </Card>
  );
};
