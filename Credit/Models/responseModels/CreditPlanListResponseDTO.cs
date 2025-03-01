using CreditService_Patterns.Models.innerModels;

namespace hitscord_net.Models.DBModels;

public class CreditPlanListResponseDTO
{
    public required List<CreditPlanResponse> PlanList { get; set; }
}