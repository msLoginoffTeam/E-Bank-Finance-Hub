import { UnstyledButton } from '@mantine/core';
import { Link } from 'react-router-dom';

import classes from '~/App.module.scss';

export const Navbar = () => {
  return (
    <Link to="/" className={classes.link}>
      <UnstyledButton className={classes.control}>Главная</UnstyledButton>
    </Link>
  );
};
