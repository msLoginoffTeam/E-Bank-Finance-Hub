import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import './index.css';

import App from './App.tsx';
import { store } from './store/store.ts';

if ('serviceWorker' in navigator) {
  navigator.serviceWorker
    .register('/firebase-messaging-sw.js')
    .then((registration) => {
      console.log('Service Worker зарегистрирован:', registration);
    })
    .catch((error) => {
      console.error('Ошибка при регистрации Service Worker:', error);
    });
}

createRoot(document.getElementById('root')!).render(
  <Provider store={store}>
    <App />
  </Provider>,
);
