using System.ComponentModel.DataAnnotations;

namespace hitscord_net.Models.requestModels;

public class GetCreditRequestDTO
{
    [Required(ErrorMessage = "Account id is required.")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format in account id.")]
    public required Guid AccountId { get; set; }

    [Required(ErrorMessage = "Credit plan id is required.")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format in credit plan id.")]
    public required Guid CreditPlanId { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(10.0, float.MaxValue, ErrorMessage = "Amount must be greater than 10.0")]
    public required float Amount { get; set; }

    [Required(ErrorMessage = "Closing date id is required.")]
    public required DateTime ClosingDate { get; set; }
}