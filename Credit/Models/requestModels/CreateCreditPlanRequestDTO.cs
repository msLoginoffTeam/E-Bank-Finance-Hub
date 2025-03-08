using System.ComponentModel.DataAnnotations;

namespace hitscord_net.Models.requestModels;

public class CreateCreditPlanRequestDTO
{
    [Required(ErrorMessage = "Plan name is required.")]
    [MinLength(1, ErrorMessage = "Plan name must have at least 1 character.")]
    [MaxLength(100, ErrorMessage = "Plan name cannot exceed 100 characters.")]
    public required string PlanName { get; set; }

    [Required(ErrorMessage = "Plan percent is required.")]
    [Range(1.0, float.MaxValue, ErrorMessage = "Plan percent must be greater than 1.0")]
    public required float PlanPercent { get; set; }
}