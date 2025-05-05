import { v4 as uuidv4 } from 'uuid';

export async function retryRequest<T>(
  fn: (idempotencyKey: string) => Promise<T>,
  retries = 10,
  delayMs = 500,
): Promise<T> {
  const idempotencyKey = uuidv4();

  async function attempt(currentRetry: number): Promise<T> {
    try {
      return await fn(idempotencyKey);
    } catch (error) {
      if (currentRetry <= 0) {
        throw error;
      }
      console.warn(
        `Ошибка запроса. Повтор через ${delayMs}мс... Осталось попыток: ${currentRetry}`,
      );
      await new Promise((resolve) => setTimeout(resolve, delayMs));

      return attempt(currentRetry - 1);
    }
  }

  return attempt(retries);
}
