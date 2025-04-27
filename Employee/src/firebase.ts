import { initializeApp } from 'firebase/app';
import { getMessaging, getToken, onMessage } from 'firebase/messaging';

const firebaseConfig = {
  apiKey: 'AIzaSyBLIk7XffiH3r4DqZ4Jno1M9mtuBt1FSig',
  authDomain: 'patterns-95ab2.firebaseapp.com',
  projectId: 'patterns-95ab2',
  storageBucket: 'patterns-95ab2.firebasestorage.app',
  messagingSenderId: '707924622826',
  appId: '1:707924622826:web:6c06c8dc93118436744693',
  measurementId: 'G-LZY5CFKNG8',
};

const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);

export { messaging, getToken, onMessage };
