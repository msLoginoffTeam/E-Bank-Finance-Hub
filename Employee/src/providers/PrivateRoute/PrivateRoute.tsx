import { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';

import { useAppSelector } from '~/hooks/redux';

export const PrivateRoute = ({ children }: { children: ReactNode }) => {
  const isLoggedIn = useAppSelector((state) => state.auth.isLoggedIn);

  return isLoggedIn ? children : <Navigate to="/auth" replace />;
};
