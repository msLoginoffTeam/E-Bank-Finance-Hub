namespace Credit_Api.Models.responseModels;

public class RatingResponseDTO
{
    public required Guid ClientId { get; set; }
    public required int Rating { get; set; }
}