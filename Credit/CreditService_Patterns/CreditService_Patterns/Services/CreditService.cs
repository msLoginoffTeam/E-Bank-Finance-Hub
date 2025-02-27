using CreditService_Patterns.Contexts;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.dbModels;
using CreditService_Patterns.Models.innerModels;
using CreditService_Patterns.Models.requestModels;
using CreditService_Patterns.Models.responseModels;
using hitscord_net.Models.DBModels;
using hitscord_net.Models.requestModels;
using Microsoft.EntityFrameworkCore;

namespace CreditService_Patterns.Services;

public class CreditService : ICreditService
{
    private readonly CreditServiceContext _creditContext;

    public CreditService(CreditServiceContext creditContext)
    {
        _creditContext = creditContext ?? throw new ArgumentNullException(nameof(creditContext));
    }

    public async Task<CreditPlanListResponseDTO> GetCreditPlanListAsync()
    {
        try
        {
            var plansList = new CreditPlanListResponseDTO
            {
                PlanList = await _creditContext.Plan
                    .Select(plan => new CreditPlanResponse
                    {
                        Id = plan.Id,
                        PlanName = plan.PlanName,
                        PlanPercent = plan.PlanPercent
                    })
                    .ToListAsync()
            };

            return plansList;
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

    public async Task<ClientCreditsListForClientResponseDTO> GetCreditsListClientAsync(Guid ClientId)
    {
        try
        {
            var creditsList = new ClientCreditsListForClientResponseDTO
            {
                CreditsList = await _creditContext.Credit
                    .Include(credit => credit.CreditPlan)
                    .Where(credit => credit.ClientId == ClientId)
                    .Select(credit => new ClientCreditForClientResponse
                    {
                        Id = credit.Id,
                        CreditPlan = new CreditPlanResponse
                        {
                            Id = credit.CreditPlan.Id,
                            PlanName = credit.CreditPlan.PlanName,
                            PlanPercent = credit.CreditPlan.PlanPercent
                        },
                        Amount = credit.Amount,
                        ClosingDate = credit.ClosingDate,
                        RemainingAmount = credit.RemainingAmount,
                        Status = credit.Status
                    })
                    .ToListAsync()
            };

            return creditsList;
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

    public async Task<ClientCreditsListForEmployeeResponseDTO> GetCreditsListEmployeeAsync()
    {
        try
        {
            var creditsList = new ClientCreditsListForEmployeeResponseDTO
            {
                CreditsList = await _creditContext.Credit
                    .Include(credit => credit.CreditPlan)
                    .Select(credit => new ClientCreditForEmployeeResponse
                    {
                        Id = credit.Id,
                        ClientId = credit.ClientId,
                        CreditPlan = new CreditPlanResponse
                        {
                            Id = credit.CreditPlan.Id,
                            PlanName = credit.CreditPlan.PlanName,
                            PlanPercent = credit.CreditPlan.PlanPercent
                        },
                        Amount = credit.Amount,
                        ClosingDate = credit.ClosingDate,
                        RemainingAmount = credit.RemainingAmount,
                        Status = credit.Status
                    })
                    .ToListAsync()
            };

            return creditsList;
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

    public async Task<CreditFullDataResponseDTO> GetCreditHistoryAsync(Guid CreditId)
    {
        try
        {
            var creditCheck = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == CreditId) != null;
            if(!creditCheck)
            {
                throw new CustomException("Credit with this Id doesn't exist.", "Get credit history", "Credit Id", 400);
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
                    PlanPercent = credit.CreditPlan.PlanPercent
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
                    Type = payment.Type
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

    public async Task<Guid> CreateCreditPlanAsync(CreateCreditPlanRequestDTO NewPlanData)
    {
        try
        {
            var creditNameCheck = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.PlanName == NewPlanData.PlanName) != null;
            if(creditNameCheck)
            {
                throw new CustomException("Credit plan with this name already exist.", "Create credit plan", "Plan name", 400);
            }

            var newPlan = new CreditPlanDbModel
            {
                PlanName = NewPlanData.PlanName,
                PlanPercent = NewPlanData.PlanPercent,
                Status = CreditPlanStatusEnum.Open
            };

            await _creditContext.Plan.AddAsync(newPlan);
            await _creditContext.SaveChangesAsync();

            return newPlan.Id;
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

    public async Task<Guid> GetCreditAsync(GetCreditRequestDTO NewCreditData)
    {
        try
        {
            var creditPlanCheck = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.Id == NewCreditData.CreditPlanId) != null;
            if (!creditPlanCheck)
            {
                throw new CustomException("Credit plan with this Id doesn't exist.", "Get credit", "Plan Id", 400);
            }

            if(NewCreditData.ClosingDate < DateTime.UtcNow)
            {
                throw new CustomException("Closing date can't be early than now.", "Get credit", "Closing date", 400);
            }

            var newCredit = new ClientCreditDbModel
            {
                CreditPlanId = NewCreditData.CreditPlanId,
                ClientId = NewCreditData.ClientId,
                Amount = NewCreditData.Amount,
                ClosingDate = NewCreditData.ClosingDate,
                RemainingAmount = NewCreditData.Amount,
                Status = ClientCreditStatusEnum.Open
            };

            await _creditContext.Credit.AddAsync(newCredit);
            await _creditContext.SaveChangesAsync();

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

    public async Task<PayOffTheLoanResultResponseDTO> PayOffTheLoanAsync(PayOffTheLoanRequestDTO paymentData)
    {
        try
        {
            var credit = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == paymentData.CreditId && credit.ClientId == paymentData.ClientId);
            if (credit == null)
            {
                throw new CustomException("Credit with this Id and client Id doesn't exist", "Pay off the loan", "Credit Id and client Id", 400);
            }

            var newPayment = new CreditPaymentDbModel
            {
                ClientCreditId = paymentData.CreditId,
                PaymentAmount = paymentData.Amount > credit.RemainingAmount ? credit.RemainingAmount : paymentData.Amount,
                PaymentDate = DateTime.UtcNow,
                Type = PaymentTypeEnum.ByClient
            };

            if(newPayment.PaymentAmount == credit.RemainingAmount)
            {
                credit.RemainingAmount = 0;
                credit.Status = ClientCreditStatusEnum.Closed;
            }
            else
            {
                credit.RemainingAmount -= newPayment.PaymentAmount;
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
}