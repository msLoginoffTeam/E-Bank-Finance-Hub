import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { retryRequest } from '~/api/retryRequest';

export function createRetryableThunk<Returned, ThunkArg>(
  typePrefix: string,
  asyncFunction: (arg: ThunkArg, idempotencyKey: string) => Promise<Returned>,
) {
  return createAsyncThunk<Returned, ThunkArg, { rejectValue: string }>(
    typePrefix,
    async (arg, { rejectWithValue }) => {
      try {
        return await retryRequest((idempotencyKey) =>
          asyncFunction(arg, idempotencyKey),
        );
      } catch (error) {
        console.error(error);

        if (error instanceof AxiosError) {
          return rejectWithValue(
            error.response?.data?.message || 'Ошибка запроса',
          );
        }

        return rejectWithValue('Произошла ошибка');
      }
    },
  );
}
