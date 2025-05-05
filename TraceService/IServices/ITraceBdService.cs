using Common.Trace;
using TraceService.Data.Models;

namespace TraceService.IServices
{
	public interface ITraceBdService
	{
		Task<bool> AddTraceElement(LogEventModel data);
		Task<List<TraceDbModel>> GetAllTracesAsync();
	}
}
