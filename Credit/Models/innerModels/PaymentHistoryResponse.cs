using System.ComponentModel.DataAnnotations;

namespace CreditService_Patterns.Models.innerModels;

public class PaymentHistoryResponse
{
    public required Guid Id { get; set; }
    public required float PaymentAmount { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required PaymentTypeEnum Type { get; set; }
    public required Guid AccountId { get; set; }
}