import { notifications } from '@mantine/notifications';
import { Check, X } from 'lucide-react';

// Хук для показа уведомлений
export function useNotification() {
  const showSuccess = (message: string) => {
    notifications.show({
      message,
      position: 'top-center',
      color: 'green',
      radius: 'md',
      autoClose: 2000,
      icon: <Check />,
    });
  };

  const showError = (message: string) => {
    notifications.show({
      title: 'Ошибка',
      message,
      position: 'top-center',
      color: 'red',
      radius: 'md',
      autoClose: 2000,
      icon: <X />,
    });
  };

  return { showSuccess, showError };
}
