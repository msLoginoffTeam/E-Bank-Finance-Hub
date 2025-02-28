import { SimpleGrid } from '@mantine/core';
import { CirclePercent, Rows3, Users, Wallet } from 'lucide-react';

import { BigActionButton } from '~/ui/BigActionButton';

export const ButtonsPanel = () => {
  return (
    <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="md" verticalSpacing="md">
      <BigActionButton icon={<Wallet />} label="Открыть счета клиентов" />
      <BigActionButton
        icon={<CirclePercent />}
        label="Управление тарифами кредитов"
      />
      <BigActionButton icon={<Users />} label="Управление сотрудниками" />
      <BigActionButton icon={<Rows3 />} label="Открыть список кредитов" />
    </SimpleGrid>
  );
};
