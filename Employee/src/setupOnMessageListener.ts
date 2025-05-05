import { onMessage } from 'firebase/messaging';

import { messaging } from './firebase';

export function setupOnMessageListener(
  showNotification: (title: string, message: string) => void,
) {
  onMessage(messaging, (payload) => {
    console.log('Получено сообщение:', payload);

    const title = payload.notification?.title ?? 'Уведомление';
    const body = payload.notification?.body ?? '';

    showNotification(title, body);
  });
}
