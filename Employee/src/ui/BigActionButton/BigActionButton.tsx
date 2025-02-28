import { Button, Flex, Group, Text } from '@mantine/core';

import { BigActionButtonProps } from './BigActionButton.types';

export const BigActionButton = ({
  icon,
  label,
  onClick,
}: BigActionButtonProps) => {
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
      onClick={onClick}
    >
      <Flex w="100%" h="100%" p="md" direction="column" justify="space-between">
        <Text>{label}</Text>
        <Group>{icon}</Group>
      </Flex>
    </Button>
  );
};
