import { Burger, Group, UnstyledButton } from '@mantine/core';
import { Link } from 'react-router-dom';

import { HeaderProps } from './Header.types';

import classes from '~/App.module.scss';
import Logo from '~/assets/Logo.png';

export const Header = ({ opened, onToggle }: HeaderProps) => {
  return (
    <Group h="100%" px="md">
      <Burger opened={opened} onClick={onToggle} hiddenFrom="sm" size="sm" />
      <Group style={{ flex: 1 }}>
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
        <Group gap={0} visibleFrom="sm">
          <Link to="/" className={classes.link}>
            <UnstyledButton className={classes.control}>Главная</UnstyledButton>
          </Link>
        </Group>
      </Group>
    </Group>
  );
};
