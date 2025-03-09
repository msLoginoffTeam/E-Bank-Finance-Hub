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
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { ChevronDown, ChevronUp } from 'lucide-react';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
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
                        // eslint-disable-next-line react/no-array-index-key
                        <Card key={index} padding="md" radius="xl" withBorder>
                          <Stack gap="xs">
                            <Group>
                              <Text fz="sm" c="dimmed">
                                {op.operationType === 'Outcome'
                                  ? 'Списание:'
                                  : 'Пополнение:'}
                              </Text>
                              <Text
                                c={
                                  op.amountInRubles < 0 ||
                                  op.operationType === 'Outcome'
                                    ? 'red'
                                    : 'teal'
                                }
                                style={{ wordBreak: 'break-all' }}
                              >
                                {`${op.amountInRubles} ₽`}
                              </Text>
                            </Group>

                            <Text fz="sm" c="dimmed">
                              {format(
                                new Date(op.time),
                                'dd MMMM yyyy, HH:mm',
                                {
                                  locale: ru,
                                },
                              )}
                            </Text>
                            <Badge color="blue" radius="sm">
                              {op.operationCategory}
                            </Badge>
                          </Stack>
                        </Card>
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
