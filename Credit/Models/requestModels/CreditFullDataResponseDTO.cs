using CreditService_Patterns.Models.innerModels;
using System.ComponentModel.DataAnnotations;

namespace CreditService_Patterns.Models.requestModels;

public class CreditFullDataResponseDTO
{
    public required Guid Id { get; set; }
    public required CreditPlanResponse CreditPlan { get; set; }
    public required float Amount { get; set; }
    public required DateTime ClosingDate { get; set; }
    public required float RemainingAmount { get; set; }
    public required ClientCreditStatusEnum Status { get; set; }
    public required List<PaymentHistoryResponse> PaymentHistory { get; set; }
}