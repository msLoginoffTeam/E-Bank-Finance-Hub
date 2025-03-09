import { Routes, Route } from 'react-router-dom';

import { Auth } from '~/modules/Auth';
import { ClientDetails } from '~/modules/ClientDetails';
import { ClientsList } from '~/modules/ClientsList';
import { Credits } from '~/modules/Credits';
import { Dashboard } from '~/modules/Dashboard';
import { Users } from '~/modules/Users';
import { PrivateRoute } from '~/providers/PrivateRoute';

export const RouterComponent = () => {
  return (
    <Routes>
      <Route
        path="/"
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        }
      />
      <Route path="/auth" element={<Auth />} />
      <Route
        path="/clients"
        element={
          <PrivateRoute>
            <ClientsList />
          </PrivateRoute>
        }
      />
      <Route
        path="/clients/:id"
        element={
          <PrivateRoute>
            <ClientDetails />
          </PrivateRoute>
        }
      />
      <Route
        path="/credits"
        element={
          <PrivateRoute>
            <Credits />
          </PrivateRoute>
        }
      />
      <Route
        path="/users"
        element={
          <PrivateRoute>
            <Users />
          </PrivateRoute>
        }
      />
    </Routes>
  );
};
