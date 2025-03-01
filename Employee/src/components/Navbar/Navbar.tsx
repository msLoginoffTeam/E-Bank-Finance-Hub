import { UnstyledButton } from '@mantine/core';
import { Link } from 'react-router-dom';

import classes from '~/App.module.scss';
import { useAppSelector } from '~/hooks/redux';

export const Navbar = () => {
  const { isLoggedIn } = useAppSelector((state) => state.auth);

  return (
    <>
      <Link to="/" className={classes.link}>
        <UnstyledButton className={classes.control}>Главная</UnstyledButton>
      </Link>
      <Link to="/clients" className={classes.link}>
        <UnstyledButton className={classes.control}>Счета</UnstyledButton>
      </Link>
      {!isLoggedIn ? (
        <Link to="/auth" className={classes.link}>
          <UnstyledButton className={classes.control}>Войти</UnstyledButton>
        </Link>
      ) : (
        <UnstyledButton className={classes.control}>
          Вы авторизованы
        </UnstyledButton>
      )}
    </>
  );
};
