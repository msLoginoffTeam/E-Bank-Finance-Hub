using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UserPreferences.Models;
using UserPreferences.Services;

namespace UserPreferences.Controllers;

[Route("api/theme/")]
[ApiController]
public class ThemeController: ControllerBase
{
    private ThemeService _themeService { get; }

    public ThemeController(ThemeService themeService)
    {
        _themeService = themeService;
    }
    
    [HttpPut]
    [Route("set")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult SetTheme(Guid userId, ThemeEnum themeEnum = ThemeEnum.Light)
    {
        
        return _themeService.SetTheme(userId, themeEnum);
    }

    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult CreateTheme([Required] Guid userId, ThemeEnum themeEnum = ThemeEnum.Light)
    {
        return _themeService.CreateSetting(userId, themeEnum);
    }

    [HttpGet]
    [Route("getTheme")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ThemeEnum> GetTheme(Guid userId)
    {
        return _themeService.GetTheme(userId);
    }
}