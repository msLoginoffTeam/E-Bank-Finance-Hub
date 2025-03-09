import { Badge, Card, Group, Stack, Text } from '@mantine/core';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';

import { Operation } from '~/store/AccountsStore';

export const OperationItem = ({
  operationType,
  amountInRubles,
  operationCategory,
  time,
}: Omit<Operation, 'creditId'>) => {
  return (
    <Card padding="md" radius="xl" withBorder>
      <Stack gap="xs">
        <Group>
          <Text fz="sm" c="dimmed">
            {operationType === 'Outcome' ? 'Списание:' : 'Пополнение:'}
          </Text>
          <Text
            c={
              amountInRubles < 0 || operationType === 'Outcome' ? 'red' : 'teal'
            }
            style={{ wordBreak: 'break-all' }}
          >
            {`${amountInRubles} ₽`}
          </Text>
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
