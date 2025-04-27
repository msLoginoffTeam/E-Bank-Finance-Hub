importScripts(
  'https://www.gstatic.com/firebasejs/10.0.0/firebase-app-compat.js',
);
importScripts(
  'https://www.gstatic.com/firebasejs/10.0.0/firebase-messaging-compat.js',
);

firebase.initializeApp({
  apiKey: 'AIzaSyBLIk7XffiH3r4DqZ4Jno1M9mtuBt1FSig',
  authDomain: 'patterns-95ab2.firebaseapp.com',
  projectId: 'patterns-95ab2',
  storageBucket: 'patterns-95ab2.firebasestorage.app',
  messagingSenderId: '707924622826',
  appId: '1:707924622826:web:6c06c8dc93118436744693',
  measurementId: 'G-LZY5CFKNG8',
});

const messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) => {
  console.log(
    '[firebase-messaging-sw.js] Received background message ',
    payload,
  );
  const notificationTitle = payload.notification.title;
  const notificationOptions = {
    body: payload.notification.body,
  };

  self.registration.showNotification(notificationTitle, notificationOptions);
});
