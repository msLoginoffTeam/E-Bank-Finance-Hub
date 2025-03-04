﻿using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using CreditService_Patterns.Contexts;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.dbModels;
using CreditService_Patterns.Models.innerModels;
using CreditService_Patterns.Models.requestModels;
using CreditService_Patterns.Models.responseModels;
using EasyNetQ;
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
                        AccountId = credit.AccountId,
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

    public async Task<CreditFullDataResponseDTO> GetCreditHistoryAsync(Guid ClientId, Guid CreditId)
    {
        try
        {
            var creditCheck = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == CreditId && credit.ClientId == ClientId) != null;
            if (!creditCheck)
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
            if (creditNameCheck)
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

    public async Task<Guid> GetCreditAsync(Guid ClientId, GetCreditRequestDTO NewCreditData)
    {
        try
        {
            var creditPlanCheck = await _creditContext.Plan.FirstOrDefaultAsync(plan => plan.Id == NewCreditData.CreditPlanId) != null;
            if (!creditPlanCheck)
            {
                throw new CustomException("Credit plan with this Id doesn't exist.", "Get credit", "Plan Id", 400);
            }

            if (NewCreditData.ClosingDate < DateTime.UtcNow)
            {
                throw new CustomException("Closing date can't be early than now.", "Get credit", "Closing date", 400);
            }

            var newCredit = new ClientCreditDbModel
            {
                CreditPlanId = NewCreditData.CreditPlanId,
                ClientId = ClientId,
                AccountId = NewCreditData.AccountId,
                Amount = NewCreditData.Amount,
                ClosingDate = NewCreditData.ClosingDate,
                RemainingAmount = NewCreditData.Amount,
                Status = ClientCreditStatusEnum.Open
            };

            await _creditContext.Credit.AddAsync(newCredit);
            await _creditContext.SaveChangesAsync();

            using (var bus = RabbitHutch.CreateBus("host=rabbitmq"))
            {
                var request = new CreditOperationRequest
                {
                    CreditId = newCredit.Id,
                    AmountInRubles = newCredit.Amount,
                    OperationType = OperationType.Income
                };

                await bus.PubSub.PublishAsync<(Guid, CreditOperationRequest)>((newCredit.AccountId, request));
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
            var credit = await _creditContext.Credit.FirstOrDefaultAsync(credit => credit.Id == paymentData.CreditId && credit.AccountId == paymentData.AccountId && credit.ClientId == ClientId);
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

            using (var bus = RabbitHutch.CreateBus("host=rabbitmq"))
            {
                var request = new CreditOperationRequest
                {
                    CreditId = credit.Id,
                    AmountInRubles = newPayment.PaymentAmount,
                    OperationType = OperationType.Outcome
                };
                
                var response = await bus.Rpc.RequestAsync<(Guid, CreditOperationRequest), ErrorResponse?>((credit.AccountId, request));

                if (response != null)
                {
                    throw new CustomException(response.message, "", "", response.status);
                }
            }

            if (newPayment.PaymentAmount == credit.RemainingAmount)
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

    public async Task PayOffTheLoanAutomaticAsync()
    {
        try
        {
            var creditsList = await _creditContext.Credit.Where(Credit => Credit.RemainingAmount > 0 && Credit.Status != ClientCreditStatusEnum.Closed).ToListAsync();

            foreach (var credit in creditsList)
            {
                ErrorResponse? response;

                var request = new CreditOperationRequest
                {
                    CreditId = credit.Id,
                    AmountInRubles = MathF.Round(credit.RemainingAmount / (credit.ClosingDate - DateTime.UtcNow).Days, 2),
                    OperationType = OperationType.Outcome
                };

                using (var bus = RabbitHutch.CreateBus("host=rabbitmq"))
                {
                    response = await bus.Rpc.RequestAsync<(Guid, CreditOperationRequest), ErrorResponse?>((credit.AccountId, request));
                }

                if (response != null)
                {
                    credit.Status = ClientCreditStatusEnum.Expired;
                    _creditContext.Credit.Update(credit);
                    await _creditContext.SaveChangesAsync();
                }
                else
                {
                    var newPayment = new CreditPaymentDbModel
                    {
                        ClientCreditId = credit.Id,
                        PaymentAmount = request.AmountInRubles,
                        PaymentDate = DateTime.UtcNow,
                        Type = PaymentTypeEnum.ByClient
                    };

                    if (newPayment.PaymentAmount == credit.RemainingAmount)
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
                credit.RemainingAmount *= 1 + (credit.CreditPlan.PlanPercent / 100);

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
}