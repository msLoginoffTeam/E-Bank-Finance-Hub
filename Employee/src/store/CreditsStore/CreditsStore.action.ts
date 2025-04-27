import { CreditsAPI } from './api';
import {
  CLOSE_CREDIT_PLAN_ACTION_NAME,
  CREATE_CREDIT_PLAN_ACTION_NAME,
  GET_CLIENT_CREDIT_HISTORY_ACTION_NAME,
  GET_CLIENT_CREDIT_RATING_ACTION_NAME,
  GET_CLIENT_CREDITS_ACTION_NAME,
  GET_CREDITS_PLANS_ACTION_NAME,
} from './CreditsStore.const';
import {
  ClientCreditForEmployeeResponse,
  CreditPlan,
  GetCredidtsPlansResponse,
  RatingResponse,
} from './CreditsStore.types';

import { createRetryableThunk } from '~/store/retryableThunk';

export const getCreditsPlans = createRetryableThunk<
  GetCredidtsPlansResponse,
  string
>(GET_CREDITS_PLANS_ACTION_NAME, async (accessToken, idempotencyKey) =>
  CreditsAPI.getCreditsPlan(accessToken, idempotencyKey),
);

export const createCreditPlan = createRetryableThunk<
  CreditPlan,
  { accessToken: string; planName: string; planPercent: number }
>(
  CREATE_CREDIT_PLAN_ACTION_NAME,
  async ({ accessToken, planName, planPercent }, idempotencyKey) =>
    CreditsAPI.createCreditsPlan({
      accessToken,
      planName,
      planPercent,
      idempotencyKey,
    }),
);

export const getClientCredits = createRetryableThunk<
  ClientCreditForEmployeeResponse[],
  { accessToken: string; id: string }
>(GET_CLIENT_CREDITS_ACTION_NAME, async ({ accessToken, id }, idempotencyKey) =>
  CreditsAPI.getClientCredits(accessToken, id, idempotencyKey),
);

export const closeCreditPlan = createRetryableThunk<
  void,
  { accessToken: string; id: string }
>(CLOSE_CREDIT_PLAN_ACTION_NAME, async ({ accessToken, id }, idempotencyKey) =>
  CreditsAPI.closeCreditsPlan(accessToken, id, idempotencyKey),
);

export const getClientCreditHistory = createRetryableThunk<
  Omit<ClientCreditForEmployeeResponse, 'clientId' | 'accountId'>,
  { accessToken: string; id: string }
>(
  GET_CLIENT_CREDIT_HISTORY_ACTION_NAME,
  async ({ accessToken, id }, idempotencyKey) =>
    CreditsAPI.getCreditHistory(accessToken, id, idempotencyKey),
);

export const getCreditRating = createRetryableThunk<
  RatingResponse,
  { accessToken: string; clientId: string }
>(
  GET_CLIENT_CREDIT_RATING_ACTION_NAME,
  async ({ accessToken, clientId }, idempotencyKey) =>
    CreditsAPI.getCreditRating(accessToken, clientId, idempotencyKey),
);
