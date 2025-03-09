import { Routes, Route } from 'react-router-dom';

import { Auth } from '~/modules/Auth';
import { ClientDetails } from '~/modules/ClientDetails';
import { ClientsList } from '~/modules/ClientsList';
import { Credits } from '~/modules/Credits';
import { Dashboard } from '~/modules/Dashboard';
import { Users } from '~/modules/Users';

export const RouterComponent = () => {
  return (
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/auth" element={<Auth />} />
      <Route path="/clients" element={<ClientsList />} />
      <Route path="/clients/:id" element={<ClientDetails />} />
      <Route path="/credits" element={<Credits />} />
      <Route path="/users" element={<Users />} />
    </Routes>
  );
};
