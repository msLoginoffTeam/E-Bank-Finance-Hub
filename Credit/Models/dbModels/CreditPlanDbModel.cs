using CreditService_Patterns.Models.innerModels;
using System.ComponentModel.DataAnnotations;

namespace CreditService_Patterns.Models.dbModels;

public class CreditPlanDbModel
{
    public CreditPlanDbModel()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    required public string PlanName { get; set; }

    [Required]
    required public float PlanPercent { get; set; }

    [Required]
    required public CreditPlanStatusEnum Status { get; set; }
}