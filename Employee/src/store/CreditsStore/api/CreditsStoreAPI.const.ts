export const GET_CREDITS_PLAN = `/Api/Credit/GetCreditPlans?ElementsNumber=1000&PageNumber=1`;
export const CREATE_CREDITS_PLAN = `/Api/Credit/CreateCreditPlan`;
export const CLOSE_CREDITS_PLAN = `/Api/Credit/CloseCreditPlan`;
export const GET_CLIENT_CREDITS = (id: string) =>
  `/Api/Credit/GetCreditsList/Employee?ClientId=${id}&ElementsNumber=1000&PageNumber=1`;
export const GET_CREDIT_HISTORY = (id: string) =>
  `/Api/Credit/GetCreditHistory/Employee?CreditId=${id}`;

export const GET_CREDIT_RATING = (id: string) =>
  `/Api/Credit/GetCreditRating?ClientId=${id}`;
