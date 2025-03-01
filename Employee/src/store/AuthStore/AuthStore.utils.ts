export const loadTokenFromLocalStorage = (
  tokenType: 'accessToken' | 'refreshToken',
) => {
  try {
    const token = localStorage.getItem(tokenType);

    return token ? token : '';
  } catch (err) {
    console.error('Could not load token', err);

    return '';
  }
};
