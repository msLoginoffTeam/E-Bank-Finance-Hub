import { notifications } from '@mantine/notifications';
import { Check, MessageSquare, X } from 'lucide-react';

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

  const showMessage = (title: string, message: string) => {
    notifications.show({
      title,
      message,
      position: 'top-right',
      color: 'yellow',
      radius: 'md',
      autoClose: 2000,
      icon: <MessageSquare />,
    });
  };

  return { showSuccess, showError, showMessage };
}
