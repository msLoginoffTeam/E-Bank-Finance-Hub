import { getToken } from 'firebase/messaging';

import { messaging } from './firebase';

const VAPID_KEY =
  'BFNcF5xntbvh6j_r03ZAOwa_1xHCCozkegfgwX1ZTG0oTpw__idiXDd9gogezYnbzCPA9uNJGffhPfq90o4GESM';

export async function requestNotificationPermission(): Promise<string | null> {
  try {
    const permission = await Notification.requestPermission();

    if (permission === 'granted') {
      const token = await getToken(messaging, { vapidKey: VAPID_KEY });

      return token;
    } else {
      console.warn('Permission not granted');

      return null;
    }
  } catch (err) {
    console.error('FCM Error:', err);

    return null;
  }
}
