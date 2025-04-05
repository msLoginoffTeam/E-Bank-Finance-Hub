using CreditService_Patterns.Models.innerModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditService_Patterns.Models.dbModels;

public class ClientCreditDbModel
{
    public ClientCreditDbModel()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CreditPlanId { get; set; }

    [ForeignKey(nameof(CreditPlanId))]
    public CreditPlanDbModel CreditPlan { get; set; }

    [Required]
    public required Guid ClientId { get; set; }

    [Required]
    public required Guid AccountId { get; set; }

    [Required]
    public required int Amount { get; set; }

    [Required]
    public required DateTime ClosingDate { get; set; }

    [Required]
    public required int RemainingAmount { get; set; }

    [Required]
    public required ClientCreditStatusEnum Status { get; set; }
}