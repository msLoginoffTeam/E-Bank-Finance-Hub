import {
  Button,
  Group,
  Modal,
  NumberInput,
  Stack,
  TextInput,
} from '@mantine/core';
import { hasLength, useForm } from '@mantine/form';
import { useEffect } from 'react';

import { CreateModalProps } from './CreateModal.types';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { useNotification } from '~/hooks/useNotification';
import { createCreditPlan, CreditPlan } from '~/store/CreditsStore';

export const CreateModal = ({ opened, close }: CreateModalProps) => {
  const { accessToken } = useAppSelector((state) => state.auth);
  const { error } = useAppSelector((state) => state.credits);
  const { showSuccess, showError } = useNotification();
  const dispatch = useAppDispatch();

  const form = useForm<Omit<CreditPlan, 'id' | 'status'>>({
    mode: 'uncontrolled',
    initialValues: {
      planName: '',
      planPercent: 0,
    },
    validate: {
      planName: hasLength({ min: 5 }, 'Название должно быть длинее 5 символов'),
    },
  });

  const handleSubmit = async () => {
    const values = form.getValues();

    if (accessToken) {
      const result = await dispatch(
        createCreditPlan({
          accessToken,
          planName: values.planName,
          planPercent: values.planPercent,
        }),
      );

      if (result.meta.requestStatus === 'fulfilled') {
        showSuccess('Кредитный план создан');
        close();
      }
    }
  };

  useEffect(() => {
    if (error) showError(error);
  }, [error]);

  return (
    <Modal
      opened={opened}
      onClose={close}
      title="Создание нового тарифа"
      radius="lg"
      centered
    >
      <form
        onSubmit={form.onSubmit(() => {
          handleSubmit();
        })}
        style={{ width: '100%' }}
      >
        <Stack gap="md">
          <TextInput
            withAsterisk
            radius="md"
            label="Название"
            placeholder="Введите название"
            key={form.key('planName')}
            {...form.getInputProps('planName')}
          />
          <NumberInput
            withAsterisk
            radius="md"
            label="Процент"
            placeholder="Выберите процент"
            suffix=" %"
            allowNegative={false}
            key={form.key('planPercent')}
            {...form.getInputProps('planPercent')}
          />
          <Group justify="center">
            <Button
              type="submit"
              variant="light"
              radius="xl"
              w="100%"
              maw={200}
            >
              Создать
            </Button>
          </Group>
        </Stack>
      </form>
    </Modal>
  );
};
