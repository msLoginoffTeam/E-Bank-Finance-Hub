import {
  CLOSE_CREDITS_PLAN,
  CREATE_CREDITS_PLAN,
  GET_CLIENT_CREDITS,
  GET_CREDIT_HISTORY,
  GET_CREDIT_RATING,
  GET_CREDITS_PLAN,
} from './CreditsStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import {
  ClientCreditForEmployeeResponse,
  CreditPlan,
  GetCredidtsPlansResponse,
  Pagination,
  RatingResponse,
} from '~/store/CreditsStore';

export const getCreditsPlan = async (
  accessToken: string,
  idempotencyKey?: string,
): Promise<GetCredidtsPlansResponse> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<GetCredidtsPlansResponse>(
    GET_CREDITS_PLAN,
    {
      baseURL: `${BASE_URL}:8081`,
      headers,
    },
  );

  return data;
};

export const createCreditsPlan = async ({
  accessToken,
  planName,
  planPercent,
  idempotencyKey,
}: {
  accessToken: string;
  planName: string;
  planPercent: number;
  idempotencyKey?: string;
}): Promise<CreditPlan> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.post<CreditPlan>(
    CREATE_CREDITS_PLAN,
    {
      planName,
      planPercent,
    },
    {
      baseURL: `${BASE_URL}:8081`,
      headers,
    },
  );

  return data;
};

export const getClientCredits = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<ClientCreditForEmployeeResponse[]> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<{
    creditsList: ClientCreditForEmployeeResponse[];
    pagination: Pagination;
  }>(GET_CLIENT_CREDITS(id), {
    baseURL: `${BASE_URL}:8081`,
    headers,
  });

  return data.creditsList;
};

export const closeCreditsPlan = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<void> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  await axiosInstance.post<void>(
    CLOSE_CREDITS_PLAN,
    {
      creditPlanId: id,
    },
    {
      baseURL: `${BASE_URL}:8081`,
      headers,
    },
  );
};

export const getCreditHistory = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<Omit<ClientCreditForEmployeeResponse, 'clientId' | 'accountId'>> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<
    Omit<ClientCreditForEmployeeResponse, 'clientId' | 'accountId'>
  >(GET_CREDIT_HISTORY(id), {
    baseURL: `${BASE_URL}:8081`,
    headers,
  });

  return data;
};

export const getCreditRating = async (
  accessToken: string,
  clientId: string,
  idempotencyKey?: string,
): Promise<RatingResponse> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<RatingResponse>(
    GET_CREDIT_RATING(clientId),
    {
      baseURL: `${BASE_URL}:8081`,
      headers,
    },
  );

  return data;
};
