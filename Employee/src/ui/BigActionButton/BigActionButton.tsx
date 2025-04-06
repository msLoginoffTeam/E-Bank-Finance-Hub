import {
  Button,
  Flex,
  Group,
  Text,
  useMantineColorScheme,
} from '@mantine/core';

import { BigActionButtonProps } from './BigActionButton.types';

export const BigActionButton = ({
  icon,
  label,
  onClick,
}: BigActionButtonProps) => {
  const { colorScheme } = useMantineColorScheme();

  return (
    <Button
      variant="white"
      radius="xl"
      size="xl"
      styles={{
        root: {
          width: '250px',
          height: '150px',
          color: '#000',
        },
        label: {
          whiteSpace: 'normal',
          width: '100%',
        },
      }}
      bg={colorScheme === 'dark' ? '#2e2e2e' : 'white'}
      c={colorScheme === 'dark' ? 'white' : 'dark'}
      onClick={onClick}
    >
      <Flex w="100%" h="100%" p="md" direction="column" justify="space-between">
        <Text>{label}</Text>
        <Group>{icon}</Group>
      </Flex>
    </Button>
  );
};
