import { Avatar, Badge, Button, Card, Group, Stack, Text } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

import { ClientItemProps } from './ClientItem.types';

export const ClientItem = ({
  id,
  fullName,
  email,
  isBlocked,
}: ClientItemProps) => {
  const navigate = useNavigate();

  return (
    <Card padding="lg" radius="xl" w="100%" maw={600}>
      <Group gap="md">
        <Avatar size="lg" name={fullName} color="initials" />
        <Stack gap="xs">
          <Group>
            <Text c="dimmed">ФИО:</Text>
            <Text style={{ wordBreak: 'break-all' }}>{fullName}</Text>
          </Group>
          <Group>
            <Text c="dimmed">Email:</Text>
            <Text>{email}</Text>
          </Group>
          {isBlocked && <Badge color="red">Заблокирован</Badge>}
          <Button
            variant="white"
            size="md"
            radius="xl"
            onClick={() => navigate(`/clients/${id}`)}
          >
            Открыть детальную информацию
          </Button>
        </Stack>
      </Group>
    </Card>
  );
};
