import { Burger, Group, UnstyledButton } from '@mantine/core';
import { Link } from 'react-router-dom';

import { HeaderProps } from './Header.types';

import classes from '~/App.module.scss';
import Logo from '~/assets/Logo.png';
import { useAppSelector } from '~/hooks/redux';

export const Header = ({ opened, onToggle }: HeaderProps) => {
  const { isLoggedIn } = useAppSelector((state) => state.auth);

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
          <Link to="/" className={classes.link}>
            <UnstyledButton className={classes.control}>Главная</UnstyledButton>
          </Link>
          <Link to="/clients" className={classes.link}>
            <UnstyledButton className={classes.control}>Счета</UnstyledButton>
          </Link>
        </Group>
        {!isLoggedIn ? (
          <Link to="/auth" className={classes.link}>
            <UnstyledButton className={classes.control}>Войти</UnstyledButton>
          </Link>
        ) : (
          <UnstyledButton className={classes.control}>
            Вы авторизованы
          </UnstyledButton>
        )}
      </Group>
    </Group>
  );
};
