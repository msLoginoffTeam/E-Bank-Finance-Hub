import { Button, Card, Group, Stack, Text } from '@mantine/core';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { useNotification } from '~/hooks/useNotification';
import {
  closeCreditPlan,
  CreditPlan,
  CreditStatus,
} from '~/store/CreditsStore';

export const CreditPlanItem = ({
  id,
  planName,
  planPercent,
  status,
}: CreditPlan) => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { showSuccess } = useNotification();

  const handleArchive = async () => {
    if (accessToken) {
      const result = await dispatch(closeCreditPlan({ accessToken, id }));

      if (result.meta.requestStatus === 'fulfilled') {
        showSuccess('Кредитный план архивирован');
      }
    }
  };

  return (
    <Card padding="lg" radius="xl" w="100%" maw={600}>
      <Stack gap="xs">
        <Group>
          <Text c="dimmed">Название:</Text>
          <Text>{planName}</Text>
        </Group>
        <Group>
          <Text c="dimmed">Процент:</Text>
          <Text>{planPercent} %</Text>
        </Group>
        <Group>
          <Text c="dimmed">Статус:</Text>
          {status === CreditStatus.Archive ? (
            <Text c="red">Архивирован</Text>
          ) : (
            <Text c="green">Активен</Text>
          )}
        </Group>
        {status !== CreditStatus.Archive && (
          <Button variant="light" radius="xl" onClick={handleArchive}>
            Архивировать тариф
          </Button>
        )}
      </Stack>
    </Card>
  );
};
