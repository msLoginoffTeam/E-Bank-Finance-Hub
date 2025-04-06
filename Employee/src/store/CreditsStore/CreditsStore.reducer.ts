import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import {
  closeCreditPlan,
  createCreditPlan,
  getClientCreditHistory,
  getClientCredits,
  getCreditRating,
  getCreditsPlans,
} from './CreditsStore.action';
import { CREDITS_SLICE_NAME } from './CreditsStore.const';
import { CreditsState, CreditStatus, Payment } from './CreditsStore.types';

const initialState: CreditsState = {
  planList: [],
  credits: [],
  rating: 0,
  isLoading: false,
  error: undefined,
};

export const CreditsSlice = createSlice({
  name: CREDITS_SLICE_NAME,
  initialState,
  reducers: {
    setPayment: (
      state,
      action: PayloadAction<{
        creditId: string;
        operation: Payment;
      }>,
    ) => {
      const credit = state.credits.find(
        (credit) => credit.id === action.payload.creditId,
      );

      if (credit) {
        credit.paymentHistory = [
          ...credit.paymentHistory,
          action.payload.operation,
        ];
        credit.remainingAmount =
          credit.remainingAmount - action.payload.operation.paymentAmount;
      }
      state.error = undefined;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getCreditsPlans.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getCreditsPlans.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.planList = payload.planList;
        state.error = undefined;
      })
      .addCase(getCreditsPlans.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(createCreditPlan.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(createCreditPlan.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.planList.push(payload);
        state.error = undefined;
      })
      .addCase(createCreditPlan.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(getClientCredits.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getClientCredits.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.credits = payload.map((credit) => ({
          ...credit,
          paymentHistory: [],
        }));
        state.error = undefined;
      })
      .addCase(getClientCredits.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(closeCreditPlan.pending, (state) => {
        state.error = undefined;
      })
      .addCase(closeCreditPlan.fulfilled, (state, { meta }) => {
        const id = meta.arg.id;
        state.planList = state.planList.map((plan) =>
          plan.id === id ? { ...plan, status: CreditStatus.Archive } : plan,
        );
        state.error = undefined;
      })
      .addCase(closeCreditPlan.rejected, (state, { payload }) => {
        state.error = payload;
      })

      .addCase(getClientCreditHistory.pending, (state, { meta }) => {
        const creditId = meta.arg.id;
        const credit = state.credits.find((credit) => credit.id === creditId);

        if (credit) {
          credit.isLoadingHistory = false;
        }
        state.error = undefined;
      })
      .addCase(getClientCreditHistory.fulfilled, (state, { payload, meta }) => {
        const creditId = meta.arg.id;
        state.isLoading = false;
        const account = state.credits.find((credit) => credit.id === creditId);

        if (account) {
          account.isLoadingHistory = true;
          account.paymentHistory = payload.paymentHistory;
        }
        state.error = undefined;
      })
      .addCase(getClientCreditHistory.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })

      .addCase(getCreditRating.pending, (state) => {
        state.error = undefined;
      })
      .addCase(getCreditRating.fulfilled, (state, { payload }) => {
        state.rating = payload.rating;
        state.error = undefined;
      })
      .addCase(getCreditRating.rejected, (state, { payload }) => {
        state.error = payload;
      });
  },
});

export const { setPayment } = CreditsSlice.actions;

export const CreditsReducer = CreditsSlice.reducer;
