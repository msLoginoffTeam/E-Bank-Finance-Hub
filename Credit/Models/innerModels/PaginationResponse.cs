namespace Credit_Api.Models.innerModels;

public class PaginationResponse
{
    public required int RequestedNumber { get; set; }
    public required int PageNumber { get; set; }
    public required int ActualNumber { get; set; }
    public required int FullCount { get; set; }
}