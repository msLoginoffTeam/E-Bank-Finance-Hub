using Credit_Api.Models.innerModels;
using CreditService_Patterns.Models.innerModels;

namespace hitscord_net.Models.DBModels;

public class CreditPlanListResponseDTO
{
    public required List<CreditPlanResponse> PlanList { get; set; }
    public required PaginationResponse Pagination { get; set; }
}