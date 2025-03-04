﻿using CreditService_Patterns.Models.dbModels;
using CreditService_Patterns.Models.requestModels;
using CreditService_Patterns.Models.responseModels;
using hitscord_net.Models.DBModels;
using hitscord_net.Models.requestModels;

namespace CreditService_Patterns.IServices;

public interface ICreditService
{
    Task<CreditPlanListResponseDTO> GetCreditPlanListAsync();
    Task<ClientCreditsListForClientResponseDTO> GetCreditsListClientAsync(Guid ClientId);
    Task<ClientCreditsListForEmployeeResponseDTO> GetCreditsListEmployeeAsync();
    Task<CreditFullDataResponseDTO> GetCreditHistoryAsync(Guid ClientId, Guid CreditId);
    Task<Guid> CreateCreditPlanAsync(CreateCreditPlanRequestDTO NewPlanData);
    Task<Guid> GetCreditAsync(Guid ClientId, GetCreditRequestDTO NewCreditData);
    Task<PayOffTheLoanResultResponseDTO> PayOffTheLoanAsync(Guid ClientId, PayOffTheLoanRequestDTO paymentData);
    Task<ClientCreditDbModel> GetCredit(Guid CreditId);

    Task PayOffTheLoanAutomaticAsync();
    Task PercentAsync();
}