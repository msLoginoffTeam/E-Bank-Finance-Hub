import { Menu, UnstyledButton } from '@mantine/core';
import { LogOut } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';

import classes from '~/App.module.scss';
import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { logout } from '~/store/AuthStore';

export const Navbar = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { isLoggedIn } = useAppSelector((state) => state.auth);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/auth');
  };

  return (
    <>
      {isLoggedIn && (
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
        </>
      )}
      {!isLoggedIn ? (
        <Link to="/auth" className={classes.link}>
          <UnstyledButton className={classes.control}>Войти</UnstyledButton>
        </Link>
      ) : (
        <Menu shadow="md" width={200}>
          <Menu.Target>
            <UnstyledButton className={classes.control}>
              Вы авторизованы
            </UnstyledButton>
          </Menu.Target>
          <Menu.Dropdown>
            <Menu.Item leftSection={<LogOut />} onClick={handleLogout}>
              Выйти
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>
      )}
    </>
  );
};
