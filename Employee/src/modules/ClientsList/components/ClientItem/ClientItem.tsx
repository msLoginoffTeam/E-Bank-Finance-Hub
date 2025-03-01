import { Avatar, Badge, Button, Card, Group, Stack, Text } from '@mantine/core';

import { ClientItemProps } from './ClientItem.types';

export const ClientItem = ({ fullName, email, isBlocked }: ClientItemProps) => {
  return (
    <Card padding="lg" radius="xl" w="100%" maw={600}>
      <Group gap="md">
        <Avatar size="lg" name={fullName} color="initials" />
        <Stack gap="xs">
          <Group>
            <Text c="dimmed">ФИО:</Text>
            <Text>{fullName}</Text>
          </Group>
          <Group>
            <Text c="dimmed">Email:</Text>
            <Text>{email}</Text>
          </Group>
          {isBlocked && <Badge color="red">Заблокирован</Badge>}
          <Button variant="white" size="md" radius="xl">
            Открыть детальную информацию
          </Button>
        </Stack>
      </Group>
    </Card>
  );
};
