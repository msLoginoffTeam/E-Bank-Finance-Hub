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
        <UnstyledButton className={classes.control}>Клиенты</UnstyledButton>
      </Link>
      <Link to="/credits" className={classes.link}>
        <UnstyledButton className={classes.control}>Кредиты</UnstyledButton>
      </Link>
      <Link to="/users" className={classes.link}>
        <UnstyledButton className={classes.control}>
          Пользователи
        </UnstyledButton>
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
