using Common.ErrorHandling;
using Common.Models;
using Common.Rabbit.DTOs.Requests;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services.Utils;
using Credit_Api.Models.innerModels;
using Credit_Api.Models.responseModels;
using CreditService_Patterns.Contexts;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.dbModels;
using CreditService_Patterns.Models.innerModels;
using CreditService_Patterns.Models.responseModels;
using CreditService_Patterns.Services.Utils;
using EasyNetQ;
using hitscord_net.Models.DBModels;
using hitscord_net.Models.requestModels;
using Microsoft.EntityFrameworkCore;

namespace CreditService_Patterns.Services;

public class CreditService : ICreditService
{
    private readonly CreditServiceContext _creditContext;
    private readonly CreditRabbit _rabbit;

    public CreditService(CreditServiceContext creditContext)
    {
        _creditContext = creditContext ?? throw new ArgumentNullException(nameof(creditContext));
    }

    public async Task<ClientCreditsListForClientResponseDTO> GetCreditsListClientAsync(Guid ClientId, ClientCreditStatusEnum? Status, int ElementsNumber, int PageNumber)
    {
        try
        {
            var requestedCredits = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .Where(credit => credit.ClientId == ClientId && (credit.Status == Status || Status == null))
                .Skip((PageNumber - 1) * ElementsNumber)
                .Take(ElementsNumber)
                .Select(credit => new ClientCreditForClientResponse
                {
                    Id = credit.Id,
                    CreditPlan = new CreditPlanResponse
                    {
                        Id = credit.CreditPlan.Id,
                        PlanName = credit.CreditPlan.PlanName,
                        PlanPercent = credit.CreditPlan.PlanPercent,
                        Status = credit.CreditPlan.Status
                    },
                    Amount = credit.Amount,
                    ClosingDate = credit.ClosingDate,
                    RemainingAmount = credit.RemainingAmount,
                    Status = credit.Status
                })
                .ToListAsync();

            var fullCount = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .Where(credit => credit.ClientId == ClientId && (credit.Status == Status || Status == null))
                .CountAsync();

            var pagination = new PaginationResponse
            {
                RequestedNumber = ElementsNumber,
                PageNumber = PageNumber,
                ActualNumber = requestedCredits.Count(),
                FullCount = fullCount
            };

            return (new ClientCreditsListForClientResponseDTO
            {
                CreditsList = requestedCredits,
                Pagination = pagination
            });
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ClientCreditsListForEmployeeResponseDTO> GetCreditsListEmployeeAsync(Guid? ClientId, ClientCreditStatusEnum? Status, int ElementsNumber, int PageNumber)
    {
        try
        {
            var requestedCredits = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .Where(credit => (credit.ClientId == ClientId || ClientId == null) && (credit.Status == Status || Status == null))
                .Skip((PageNumber - 1) * ElementsNumber)
                .Take(ElementsNumber)
                .Select(credit => new ClientCreditForEmployeeResponse
                {
                    Id = credit.Id,
                    ClientId = credit.ClientId,
                    AccountId = credit.AccountId,
                    CreditPlan = new CreditPlanResponse
                    {
                        Id = credit.CreditPlan.Id,
                        PlanName = credit.CreditPlan.PlanName,
                        PlanPercent = credit.CreditPlan.PlanPercent,
                        Status = credit.CreditPlan.Status
                    },
                    Amount = credit.Amount,
                    ClosingDate = credit.ClosingDate,
                    RemainingAmount = credit.RemainingAmount,
                    Status = credit.Status
                })
                .ToListAsync();

            var fullCount = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .Where(credit => (credit.ClientId == ClientId || ClientId == null) && (credit.Status == Status || Status == null))
                .CountAsync();

            var pagination = new PaginationResponse
            {
                RequestedNumber = ElementsNumber,
                PageNumber = PageNumber,
                ActualNumber = requestedCredits.Count(),
                FullCount = fullCount
            };

            return (new ClientCreditsListForEmployeeResponseDTO
            {
                CreditsList = requestedCredits,
                Pagination = pagination
            });
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CreditFullDataResponseDTO> GetCreditHistoryForClientAsync(Guid ClientId, Guid CreditId)
    {
        try
        {
            var creditCheck = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == CreditId && credit.ClientId == ClientId) != null;
            if (!creditCheck)
            {
                throw new CustomException($"Credit with {CreditId} Id doesn't exist.", "Get credit history for client", "Credit Id", 400);
            }

            var credit = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .FirstOrDefaultAsync(credit => credit.Id == CreditId);
            var payments = await _creditContext.Payment.Where(payment => payment.ClientCreditId == CreditId).ToListAsync();

            var creditData = new CreditFullDataResponseDTO
            {
                Id = credit.Id,
                CreditPlan = new CreditPlanResponse
                {
                    Id = credit.CreditPlan.Id,
                    PlanName = credit.CreditPlan.PlanName,
                    PlanPercent = credit.CreditPlan.PlanPercent,
                    Status = credit.CreditPlan.Status
                },
                Amount = credit.Amount,
                ClosingDate = credit.ClosingDate,
                RemainingAmount = credit.RemainingAmount,
                Status = credit.Status,
                PaymentHistory = payments.Select(payment => new PaymentHistoryResponse
                {
                    Id = payment.Id,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate,
                    Type = payment.Type,
                    AccountId = payment.AccountId,
                })
                .ToList()
            };

            return creditData;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CreditFullDataResponseDTO> GetCreditHistoryForEmployeeAsync(Guid CreditId)
    {
        try
        {
            var creditCheck = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == CreditId) != null;
            if (!creditCheck)
            {
                throw new CustomException($"Credit with {CreditId} Id doesn't exist.", "Get credit history for employee", "Credit Id", 400);
            }

            var credit = await _creditContext.Credit
                .Include(credit => credit.CreditPlan)
                .FirstOrDefaultAsync(credit => credit.Id == CreditId);
            var payments = await _creditContext.Payment.Where(payment => payment.ClientCreditId == CreditId).ToListAsync();

            var creditData = new CreditFullDataResponseDTO
            {
                Id = credit.Id,
                CreditPlan = new CreditPlanResponse
                {
                    Id = credit.CreditPlan.Id,
                    PlanName = credit.CreditPlan.PlanName,
                    PlanPercent = credit.CreditPlan.PlanPercent,
                    Status = credit.CreditPlan.Status
                },
                Amount = credit.Amount,
                ClosingDate = credit.ClosingDate,
                RemainingAmount = credit.RemainingAmount,
                Status = credit.Status,
                PaymentHistory = payments.Select(payment => new PaymentHistoryResponse
                {
                    Id = payment.Id,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate,
                    Type = payment.Type,
                    AccountId = payment.AccountId,
                })
                .ToList()
            };

            return creditData;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CreditPlanListResponseDTO> GetCreditPlanListAsync(string Role, int ElementsNumber, int PageNumber)
    {
        try
        {
            var plansList = await _creditContext.Plan
                .Where(plan => Role != "Client" || plan.Status != CreditPlanStatusEnum.Archive)
                .Skip((PageNumber - 1) * ElementsNumber)
                .Take(ElementsNumber)
                .Select(plan => new CreditPlanResponse
                {
                    Id = plan.Id,
                    PlanName = plan.PlanName,
                    PlanPercent = plan.PlanPercent,
                    Status = Role == "Client" ? null : plan.Status
                })
                .ToListAsync();

            var fullCount = await _creditContext.Plan
                .Where(plan => Role != "Client" || plan.Status != CreditPlanStatusEnum.Archive)
                .CountAsync();

            var pagination = new PaginationResponse
            {
                RequestedNumber = ElementsNumber,
                PageNumber = PageNumber,
                ActualNumber = plansList.Count(),
                FullCount = fullCount
            };

            return (new CreditPlanListResponseDTO
            {
                PlanList = plansList,
                Pagination = pagination
            });
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CreditPlanResponse> CreateCreditPlanAsync(CreateCreditPlanRequestDTO NewPlanData)
    {
        try
        {
            var creditNameCheck = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.PlanName == NewPlanData.PlanName) != null;
            if (creditNameCheck)
            {
                throw new CustomException($"Credit plan with {NewPlanData.PlanName} name already exist.", "Create credit plan", "Plan name", 400);
            }

            var newPlan = new CreditPlanDbModel
            {
                PlanName = NewPlanData.PlanName,
                PlanPercent = NewPlanData.PlanPercent,
                Status = CreditPlanStatusEnum.Open
            };

            await _creditContext.Plan.AddAsync(newPlan);
            await _creditContext.SaveChangesAsync();

            return (new CreditPlanResponse
            {
                Id = newPlan.Id,
                PlanName = newPlan.PlanName,
                PlanPercent = newPlan.PlanPercent,
                Status = newPlan.Status
            });
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Guid> CloseCreditPlanAsync(Guid CreditPlanId)
    {
        try
        {
            var creditPlan = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.Id == CreditPlanId);
            if (creditPlan == null)
            {
                throw new CustomException($"Credit plan with {CreditPlanId} Id doesn't exist.", "Close credit plan", "Credit plan ID", 400);
            }
            if (creditPlan.Status == CreditPlanStatusEnum.Archive)
            {
                throw new CustomException($"Credit plan with {CreditPlanId} Id already closed.", "Close credit plan", "Credit plan ID", 400);
            }

            creditPlan.Status = CreditPlanStatusEnum.Archive;

            _creditContext.Plan.Update(creditPlan);
            await _creditContext.SaveChangesAsync();

            return creditPlan.Id;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Guid> GetCreditAsync(Guid ClientId, GetCreditRequestDTO NewCreditData)
    {
        try
        {
            var creditPlanCheck = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.Id == NewCreditData.CreditPlanId);
            if (creditPlanCheck == null)
            {
                throw new CustomException($"Credit plan with {NewCreditData.CreditPlanId} Id doesn't exist.", "Get credit", "Plan Id", 400);
            }
            if (creditPlanCheck.Status == CreditPlanStatusEnum.Archive)
            {
                throw new CustomException("Taking out a loan for an archive plan is prohibited.", "Get credit", "Plan status", 400);
            }

            if (NewCreditData.ClosingDate < DateTime.UtcNow)
            {
                throw new CustomException("The loan closing date cannot be earlier than one day after it was taken out.", "Get credit", "Closing date", 400);
            }


            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                var AccountExists = await bus.Rpc.RequestAsync<(Guid AccountId, Guid ClientId), bool>((NewCreditData.AccountId, ClientId), x => x.WithQueueName("AccountExistCheck"));
                if (!AccountExists) throw new CustomException($"Account with {NewCreditData.AccountId} doesn't exist.", "Get credit", "AccountId", 400);
            }

            var newCredit = new ClientCreditDbModel
            {
                CreditPlanId = NewCreditData.CreditPlanId,
                ClientId = ClientId,
                AccountId = NewCreditData.AccountId,
                Amount = NewCreditData.Amount,
                ClosingDate = NewCreditData.ClosingDate,
                RemainingAmount = (int)(NewCreditData.Amount * (1 + (creditPlanCheck.PlanPercent/100))),
                Status = ClientCreditStatusEnum.Open
            };

            await _creditContext.Credit.AddAsync(newCredit);
            await _creditContext.SaveChangesAsync();

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                var request = new CreditOperationRequest
                {
                    AccountId = newCredit.AccountId,
                    ClientId = newCredit.ClientId,
                    CreditId = newCredit.Id,
                    Amount = newCredit.Amount,
                    OperationType = OperationType.Income.ToString()
                };
                var response = await bus.Rpc.RequestAsync<RabbitOperationRequest, ErrorResponse>(request);
            }

            return newCredit.Id;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PayOffTheLoanResultResponseDTO> PayOffTheLoanAsync(Guid ClientId, PayOffTheLoanRequestDTO paymentData)
    {
        try
        {
            var credit = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == paymentData.CreditId && credit.ClientId == ClientId);
            if (credit == null)
            {
                throw new CustomException($"Credit with {paymentData.CreditId} Id doesn't exist", "Pay off the loan", "Credit Id", 400);
            }

            var newPayment = new CreditPaymentDbModel
            {
                ClientCreditId = paymentData.CreditId,
                PaymentAmount = paymentData.Amount > credit.RemainingAmount ? credit.RemainingAmount : paymentData.Amount,
                PaymentDate = DateTime.UtcNow,
                Type = PaymentTypeEnum.ByClient,
                AccountId = paymentData.AccountId
            };

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                var request = new CreditOperationRequest
                {
                    AccountId = paymentData.AccountId,
                    ClientId = ClientId,
                    CreditId = credit.Id,
                    Amount = newPayment.PaymentAmount,
                    OperationType = OperationType.Outcome.ToString(),
                    Type = Common.Models.CreditOperationType.ByUser
                };

                var response = await bus.Rpc.RequestAsync<RabbitOperationRequest, ErrorResponse>(request);

                if (response != null)
                {
                    throw new CustomException(response.message, "", "", response.status);
                }
            }

            credit.RemainingAmount -= newPayment.PaymentAmount;

            if (credit.RemainingAmount == 0)
            {
                credit.Status = ClientCreditStatusEnum.Closed;
            }

            await _creditContext.Payment.AddAsync(newPayment);
            _creditContext.Credit.Update(credit);
            await _creditContext.SaveChangesAsync();

            var paymentResult = new PayOffTheLoanResultResponseDTO
            {
                Id = newPayment.Id,
                CreditId = newPayment.ClientCreditId,
                PaymentDate = newPayment.PaymentDate,
                Ammount = newPayment.PaymentAmount,
                Excess = credit.RemainingAmount == 0 ? paymentData.Amount - newPayment.PaymentAmount : 0,
                CreditStatus = credit.Status
            };

            return paymentResult;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }



    public async Task PayOffTheLoanAutomaticAsync()
    {
        try
        {
            var creditsList = await _creditContext.Credit.Where(Credit => Credit.RemainingAmount > 0 && Credit.Status != ClientCreditStatusEnum.Closed).ToListAsync();

            foreach (var credit in creditsList)
            {
                if (credit.ClosingDate < DateTime.UtcNow)
                {
                    credit.Status = ClientCreditStatusEnum.Expired;
                }
                ErrorResponse? response;

                int PaymentAmount = (int)(credit.Status == ClientCreditStatusEnum.Expired
                    ?
                        MathF.Round(credit.RemainingAmount / 10)
                    :
                        MathF.Round(credit.RemainingAmount / (int)Math.Ceiling((credit.ClosingDate - DateTime.UtcNow).TotalSeconds / 20.0), 2));

                PaymentAmount = PaymentAmount > credit.RemainingAmount ? credit.RemainingAmount : PaymentAmount;

                var request = new CreditOperationRequest
                {
                    AccountId = credit.AccountId,
                    ClientId = credit.ClientId,
                    CreditId = credit.Id,
                    Amount = PaymentAmount,
                    OperationType = OperationType.Outcome.ToString(),
                    Type = Common.Models.CreditOperationType.Automatic
                };

                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    response = await bus.Rpc.RequestAsync<RabbitOperationRequest, ErrorResponse>(request);
                }

                if (response != null)
                {
                    credit.Status = credit.Status == ClientCreditStatusEnum.Expired ? ClientCreditStatusEnum.Expired : ClientCreditStatusEnum.DoublePercentage;
                    _creditContext.Credit.Update(credit);
                    await _creditContext.SaveChangesAsync();
                }
                else
                {
                    credit.Status = credit.Status == ClientCreditStatusEnum.Expired ? ClientCreditStatusEnum.Expired : ClientCreditStatusEnum.Open;
                    var newPayment = new CreditPaymentDbModel
                    {
                        ClientCreditId = credit.Id,
                        PaymentAmount = request.Amount,
                        PaymentDate = DateTime.UtcNow,
                        Type = PaymentTypeEnum.Automatic,
                        AccountId = credit.AccountId
                    };

                    credit.RemainingAmount -= newPayment.PaymentAmount;

                    if (credit.RemainingAmount == 0)
                    {
                        credit.Status = ClientCreditStatusEnum.Closed;
                    }

                    await _creditContext.Payment.AddAsync(newPayment);
                    _creditContext.Credit.Update(credit);
                    await _creditContext.SaveChangesAsync();
                }
            }
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task PercentAsync()
    {
        try
        {
            var creditsList = await _creditContext.Credit.Where(Credit => Credit.RemainingAmount > 0 && Credit.Status != ClientCreditStatusEnum.Closed).Include(credit => credit.CreditPlan).ToListAsync();

            foreach (var credit in creditsList)
            {
                if (credit.Status == ClientCreditStatusEnum.Open)
                {
                    credit.RemainingAmount = (int)(credit.RemainingAmount * (1 + (credit.CreditPlan.PlanPercent / 100)));
                }
                else
                {
                    credit.RemainingAmount = (int)(credit.RemainingAmount * (1 + ((credit.CreditPlan.PlanPercent / 100) * 2)));
                }

                _creditContext.Credit.Update(credit);
                await _creditContext.SaveChangesAsync();
            }
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ClientCreditDbModel> GetCredit(Guid CreditId)
    {
        try
        {
            return await _creditContext.Credit.FirstAsync(Credit => Credit.Id == CreditId);
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool CheckIfHaveActiveCreditAsync(Guid AccountId)
    {
        try
        {
            var creditsList = _creditContext.Credit
                    .Include(credit => credit.CreditPlan)
                    .Where(credit => credit.AccountId == AccountId && credit.Status != ClientCreditStatusEnum.Closed)
                    .Count();

            return creditsList > 0;
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message, ex.Type, ex.Object, ex.Code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RatingResponseDTO> GetRatingAsync(Guid ClientId)
    {
        var rating = new RatingResponseDTO
        {
            ClientId = ClientId,
            Rating = 0
        };
        using (var bus = RabbitHutch.CreateBus("host=localhost"))
        {
            var response = await bus.Rpc.RequestAsync<GetRatingRequest, int>(new GetRatingRequest() { ClientId = ClientId }, configure: x => x.WithQueueName("GetRating"));
            if(response == -1)
            {
                throw new CustomException("Account not found", "GetRatingAsync", "AccountId", 404);
            }
            rating.Rating = (int)response;
        }
        return rating;
    }
}