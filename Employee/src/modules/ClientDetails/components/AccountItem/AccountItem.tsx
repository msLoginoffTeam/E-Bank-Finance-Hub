import {
  Button,
  Card,
  Collapse,
  Group,
  Loader,
  ScrollArea,
  Stack,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { ChevronDown, ChevronUp } from 'lucide-react';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { OperationItem } from '~/modules/ClientDetails/components/OperationItem';
import { Account, getAccountOperations } from '~/store/AccountsStore';

export const AccountItem = ({
  id,
  name,
  balanceInRubles,
  isClosed,
  operations,
  isLoadingOperations,
}: Account) => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const [opened, { toggle }] = useDisclosure(false);

  const handleToggle = () => {
    if (!isLoadingOperations && accessToken) {
      dispatch(getAccountOperations({ accessToken, id }));
    }
    toggle();
  };

  return (
    <Card padding="md" radius="xl" shadow="md" withBorder>
      <Stack gap="md">
        <Group>
          <Text c="dimmed">Название счета:</Text>
          <Text>{name}</Text>
        </Group>
        <Group>
          <Text c="dimmed">Баланс:</Text>
          <Text>{balanceInRubles} ₽</Text>
        </Group>
        <Group>
          <Text c="dimmed">Статус счёта:</Text>
          <Text>{isClosed ? 'Закрыт' : 'Активен'}</Text>
        </Group>
        <Button
          variant="light"
          radius="xl"
          rightSection={opened ? <ChevronUp /> : <ChevronDown />}
          onClick={handleToggle}
        >
          Раскрыть операции
        </Button>
        <Collapse in={opened}>
          <Stack gap="xs">
            {!isLoadingOperations ? (
              <Loader color="blue" />
            ) : (
              <>
                {operations.length === 0 ? (
                  <Text c="dimmed">Нет операций</Text>
                ) : (
                  <ScrollArea.Autosize mah={300} p="md">
                    <Stack gap="xs">
                      {operations.map((op, index) => (
                        <OperationItem
                          // eslint-disable-next-line react/no-array-index-key
                          key={index}
                          operationType={op.operationType}
                          operationCategory={op.operationCategory}
                          amountInRubles={op.amountInRubles}
                          time={op.time}
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
