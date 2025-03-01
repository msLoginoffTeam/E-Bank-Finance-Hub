import { SimpleGrid } from '@mantine/core';
import { CirclePercent, Rows3, Users, Wallet } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

import { BigActionButton } from '~/ui/BigActionButton';

export const ButtonsPanel = () => {
  const navigate = useNavigate();

  return (
    <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="md" verticalSpacing="md">
      <BigActionButton
        icon={<Wallet />}
        label="Открыть счета клиентов"
        onClick={() => navigate('/clients')}
      />
      <BigActionButton
        icon={<CirclePercent />}
        label="Управление тарифами кредитов"
      />
      <BigActionButton icon={<Users />} label="Управление сотрудниками" />
      <BigActionButton
        icon={<Rows3 />}
        label="Открыть список кредитов клиентов"
        onClick={() => navigate('/clients')}
      />
    </SimpleGrid>
  );
};
