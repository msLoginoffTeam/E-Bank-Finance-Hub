import { Routes, Route } from 'react-router-dom';

import { Auth } from '~/modules/Auth';
import { ClientsList } from '~/modules/ClientsList';
import { Dashboard } from '~/modules/Dashboard';

export const RouterComponent = () => {
  return (
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/auth" element={<Auth />} />
      <Route path="/clients" element={<ClientsList />} />
    </Routes>
  );
};
