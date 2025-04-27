namespace TraceService.Data.Models;

public class TraceDbModel
{
	public Guid Id { get; set; }
	public string TraceId { get; set; }
	public string Event { get; set; }
	public DateTime Timestamp { get; set; }
	public string Message { get; set; }
	public bool? Success { get; set; }
	public int? Status { get; set; }
	public long? Ms { get; set; }
	public string? RequestHeaders { get; set; }
	public string? RequestBody { get; set; }
	public string? ResponseBody { get; set; }
}
