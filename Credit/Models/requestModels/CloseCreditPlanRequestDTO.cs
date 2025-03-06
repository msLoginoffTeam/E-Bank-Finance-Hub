using System.ComponentModel.DataAnnotations;

namespace Credit_Api.Models.requestModels
{
    public class CloseCreditPlanRequestDTO
    {
        [Required(ErrorMessage = "Credit plan id is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format in credit plan id.")]
        public required Guid CreditPlanId { get; set; }
    }
}
