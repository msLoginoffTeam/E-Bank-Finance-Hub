using System.ComponentModel.DataAnnotations;

namespace CreditService_Patterns.Models.innerModels;

public class CreditPlanResponse
{
    public required Guid Id { get; set; }
    public required string PlanName { get; set; }
    public required float PlanPercent { get; set; }
    public CreditPlanStatusEnum? Status { get; set; }
}