import { Routes, Route } from 'react-router-dom';

import { Dashboard } from '~/modules/Dashboard';

export const RouterComponent = () => {
  return (
    <Routes>
      <Route path="/" element={<Dashboard />} />
    </Routes>
  );
};
