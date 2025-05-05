import {
  Badge,
  Button,
  Card,
  Collapse,
  Group,
  Loader,
  ScrollArea,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { ChevronDown, ChevronUp } from 'lucide-react';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { PaymentItem } from '~/modules/ClientDetails/components/PaymentItem';
import {
  ClientCreditForEmployeeResponse,
  ClientCreditStatus,
  getClientCreditHistory,
} from '~/store/CreditsStore';

export const CreditItem = ({
  id,
  creditPlan,
  amount,
  closingDate,
  remainingAmount,
  status,
  paymentHistory,
  isLoadingHistory,
}: Omit<ClientCreditForEmployeeResponse, 'clientId' | 'accountId'>) => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const [opened, { toggle }] = useDisclosure(false);

  const handleToggle = () => {
    if (!isLoadingHistory && accessToken) {
      dispatch(getClientCreditHistory({ accessToken, id }));
    }
    toggle();
  };

  return (
    <Card padding="md" radius="xl" shadow="md" withBorder>
      <Stack gap="md">
        <Title order={5}>Кредит</Title>
        {id}
        <Group>
          <Text c="dimmed">Кредитный план:</Text>
          <Text>{creditPlan.planName}</Text>
        </Group>
        <Group>
          <Text c="dimmed">Процент:</Text>
          <Text>{creditPlan.planPercent} %</Text>
        </Group>
        <Group>
          <Text c="dimmed">Сумма:</Text>
          <Text>{amount} ₽</Text>
        </Group>
        <Group>
          <Text c="dimmed">Дата закрытия:</Text>
          <Text>
            {format(new Date(closingDate), 'dd MMMM yyyy, HH:mm', {
              locale: ru,
            })}
          </Text>
        </Group>
        <Group>
          <Text c="dimmed">Оставшаяся сумма:</Text>
          <Text>{remainingAmount} ₽</Text>
        </Group>
        <Group>
          <Text c="dimmed">Статус кредита:</Text>
          <Badge color={status === ClientCreditStatus.Expired ? 'red' : 'blue'}>
            {status === ClientCreditStatus.Closed && 'Закрыт'}
            {status === ClientCreditStatus.Open && 'Открыт'}
            {status === ClientCreditStatus.DoublePercentage &&
              'Двойной процент'}
            {status === ClientCreditStatus.Expired && 'Просрочен'}
          </Badge>
        </Group>
        <Button
          variant="light"
          radius="xl"
          rightSection={opened ? <ChevronUp /> : <ChevronDown />}
          onClick={handleToggle}
        >
          Раскрыть списания
        </Button>
        <Collapse in={opened}>
          <Stack gap="xs">
            {!isLoadingHistory ? (
              <Loader color="blue" />
            ) : (
              <>
                {paymentHistory.length === 0 ? (
                  <Text c="dimmed">Нет операций</Text>
                ) : (
                  <ScrollArea.Autosize mah={300} p="md">
                    <Stack gap="xs">
                      {paymentHistory.map((op, index) => (
                        <PaymentItem
                          // eslint-disable-next-line react/no-array-index-key
                          key={index}
                          paymentAmount={op.paymentAmount}
                          paymentDate={op.paymentDate}
                        />
                      ))}
                    </Stack>
                  </ScrollArea.Autosize>
                )}
              </>
            )}
          </Stack>
        </Collapse>
      </Stack>
    </Card>
  );
};
