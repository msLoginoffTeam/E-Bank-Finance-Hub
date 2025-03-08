using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CreditService_Patterns.Models.innerModels;

[JsonConverter(typeof(StringEnumConverter))]
public enum CreditPlanStatusEnum
{
    Open,
    Archive
}