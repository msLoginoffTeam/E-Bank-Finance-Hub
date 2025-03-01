using System.ComponentModel.DataAnnotations;

namespace hitscord_net.Models.requestModels;

public class PayOffTheLoanRequestDTO
{
    [Required(ErrorMessage = "Account id is required.")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format in account id.")]
    public required Guid AccountId { get; set; }

    [Required(ErrorMessage = "Credit id is required.")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format in credit id.")]
    public required Guid CreditId { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.0, float.MaxValue, ErrorMessage = "Amount must be greater than 0.0")]
    public required float Amount { get; set; }
}