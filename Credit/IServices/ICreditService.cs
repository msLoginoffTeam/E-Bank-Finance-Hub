using Credit_Api.Models.responseModels;
using CreditService_Patterns.Models.dbModels;
using CreditService_Patterns.Models.innerModels;
using CreditService_Patterns.Models.responseModels;
using hitscord_net.Models.DBModels;
using hitscord_net.Models.requestModels;
using UserApi.Data.Models;

namespace CreditService_Patterns.IServices;

public interface ICreditService
{
    Task<ClientCreditsListForClientResponseDTO> GetCreditsListClientAsync(Guid ClientId, ClientCreditStatusEnum? Status, int ElementsNumber, int PageNumber);
    Task<ClientCreditsListForEmployeeResponseDTO> GetCreditsListEmployeeAsync(Guid? ClientId, ClientCreditStatusEnum? Status, int ElementsNumber, int PageNumber);
    Task<CreditFullDataResponseDTO> GetCreditHistoryForClientAsync(Guid ClientId, Guid CreditId);
    Task<CreditFullDataResponseDTO> GetCreditHistoryForEmployeeAsync(Guid CreditId);
    Task<CreditPlanListResponseDTO> GetCreditPlanListAsync(string Role, int ElementsNumber, int PageNumber);
    Task<CreditPlanResponse> CreateCreditPlanAsync(CreateCreditPlanRequestDTO NewPlanData);
    Task<Guid> CloseCreditPlanAsync(Guid CreditPlanId);
    Task<Guid> GetCreditAsync(Guid ClientId, GetCreditRequestDTO NewCreditData);
    Task<PayOffTheLoanResultResponseDTO> PayOffTheLoanAsync(Guid ClientId, PayOffTheLoanRequestDTO paymentData);



    Task PayOffTheLoanAutomaticAsync();
    Task PercentAsync();
    Task<ClientCreditDbModel> GetCredit(Guid CreditId);
    Task<bool> CheckIfHaveActiveCreditAsync(Guid AccountId);
}