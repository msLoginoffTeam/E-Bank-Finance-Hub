import { Button, Stack, Title } from '@mantine/core';

export const Auth = () => {
  const redirectToAuthService = (
    isClient: boolean = false,
    returnUrl: string = 'http://localhost:5173/bank',
  ) => {
    const authServiceUrl = `http://localhost:8083?IsClient=${isClient}&returnUrl=${encodeURIComponent(returnUrl)}`;
    window.location.href = authServiceUrl;
  };

  return (
    <Stack gap="md" align="center" w="100%">
      <Title>Вход в аккаунт</Title>
      <Button
        type="submit"
        variant="light"
        radius="xl"
        w="100%"
        maw={200}
        onClick={() => redirectToAuthService()}
      >
        Войти
      </Button>
    </Stack>
  );
};
