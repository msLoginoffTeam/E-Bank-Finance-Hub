import { Button, Stack, Title, Text } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Plus } from 'lucide-react';
import { useEffect } from 'react';

import { CreateModal } from './components/CreateModal';
import { CreditPlanItem } from './components/CreditPlanItem';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { getCreditsPlans } from '~/store/CreditsStore';

export const Credits = () => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { planList } = useAppSelector((state) => state.credits);
  const [opened, { open, close }] = useDisclosure(false);

  useEffect(() => {
    if (accessToken) {
      dispatch(getCreditsPlans(accessToken));
    }
  }, [accessToken]);

  return (
    <>
      <Stack p="md" gap="xl" align="center" w="100%">
        <Title order={2}>Управление тарифами кредитов</Title>
        <Button
          variant="light"
          radius="xl"
          rightSection={<Plus />}
          onClick={open}
        >
          Создание тарифа
        </Button>
        {planList.length === 0 ? (
          <Text c="dimmed">Пока нет тарифных планов</Text>
        ) : (
          planList.map((plan) => (
            <CreditPlanItem
              key={plan.id}
              id={plan.id}
              planName={plan.planName}
              planPercent={plan.planPercent}
              status={plan.status}
            />
          ))
        )}
      </Stack>
      <CreateModal opened={opened} close={close} />
    </>
  );
};
