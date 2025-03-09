import { Burger, Group, Menu, UnstyledButton } from '@mantine/core';
import { LogOut } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';

import { HeaderProps } from './Header.types';

import classes from '~/App.module.scss';
import Logo from '~/assets/Logo.png';
import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { logout } from '~/store/AuthStore';

export const Header = ({ opened, onToggle }: HeaderProps) => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { isLoggedIn } = useAppSelector((state) => state.auth);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/auth');
  };

  return (
    <Group h="100%" px="md">
      <Burger opened={opened} onClick={onToggle} hiddenFrom="sm" size="sm" />
      <Group style={{ flex: 1 }} justify="space-between">
        <Group gap="md" visibleFrom="sm">
          <img
            src={Logo}
            alt="Logo"
            style={{
              width: 50,
              height: 50,
              borderRadius: '50%',
              margin: 2,
            }}
          />
          {isLoggedIn && (
            <>
              <Link to="/" className={classes.link}>
                <UnstyledButton className={classes.control}>
                  Главная
                </UnstyledButton>
              </Link>
              <Link to="/clients" className={classes.link}>
                <UnstyledButton className={classes.control}>
                  Клиенты
                </UnstyledButton>
              </Link>
              <Link to="/credits" className={classes.link}>
                <UnstyledButton className={classes.control}>
                  Кредиты
                </UnstyledButton>
              </Link>
              <Link to="/users" className={classes.link}>
                <UnstyledButton className={classes.control}>
                  Пользователи
                </UnstyledButton>
              </Link>
            </>
          )}
        </Group>
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
      </Group>
    </Group>
  );
};
