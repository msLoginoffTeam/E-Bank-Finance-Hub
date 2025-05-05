using Microsoft.AspNetCore.Mvc;
using TraceService.Data.Models;
using TraceService.IServices;

public class TraceController : Controller
{
	private readonly ITraceBdService _traceBdService;
	private readonly ILogger<TraceController> _logger;

	public TraceController(ITraceBdService traceBdService, ILogger<TraceController> logger)
	{
		_traceBdService = traceBdService;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetLogs()
	{
		try
		{
			var logs = await _traceBdService.GetAllTracesAsync();
			if (logs == null || !logs.Any())
			{
				ViewData["Message"] = "No logs found.";
			}
			return View(logs);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while fetching logs.");
			ViewData["Message"] = "An error occurred while fetching logs.";
			return View(new List<TraceDbModel>());
		}
	}

	[HttpPost]
	public async Task<IActionResult> RefreshLogs()
	{
		try
		{
			var logs = await _traceBdService.GetAllTracesAsync();
			return View("GetLogs", logs);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while refreshing logs.");
			ViewData["Message"] = "An error occurred while refreshing logs.";
			return View("GetLogs", new List<TraceDbModel>());
		}
	}

	[HttpGet]
	public async Task<IActionResult> GetLogsData()
	{
		var logs = await _traceBdService.GetAllTracesAsync();
		return Json(logs);
	}
}
