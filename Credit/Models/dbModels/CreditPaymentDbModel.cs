using CreditService_Patterns.Models.innerModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditService_Patterns.Models.dbModels;

public class CreditPaymentDbModel
{
    public CreditPaymentDbModel()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ClientCreditId { get; set; }

    [ForeignKey(nameof(ClientCreditId))]
    public ClientCreditDbModel ClientCredit { get; set; }

    [Required]
    public required float PaymentAmount { get; set; }

    [Required]
    public required DateTime PaymentDate { get; set; }

    [Required]
    public required PaymentTypeEnum Type { get; set; }
}