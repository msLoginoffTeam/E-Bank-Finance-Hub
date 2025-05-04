using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UserPreferences.Models;

namespace UserPreferences.Services;

public class ThemeService
{
    private readonly AppDbContext _db;
    public ThemeService(AppDbContext db) => _db = db;

    public ActionResult CreateSetting(Guid userId, ThemeEnum themeEnum)
    {
        var existingSettingItem = _db.UserSettings.FirstOrDefault(x => x.UserId == userId);
        if (existingSettingItem != null)
        {
            return new BadRequestResult();
        }
        var newSettingItem = new UserSettings(userId, themeEnum);
        _db.Add(newSettingItem);
        _db.SaveChanges();
        return new OkResult();
    }
    
    public ActionResult SetTheme(Guid userId, ThemeEnum themeEnum)
    {
        var user = _db.UserSettings.FirstOrDefault(u => u.UserId == userId);

        if (user == null)
        {
            return new NotFoundResult();
        }
        
        user.Theme = themeEnum;
        _db.SaveChanges();

        return new OkResult();

    }

    public ActionResult<ThemeEnum> GetTheme(Guid userId)
    {
        var user = _db.UserSettings.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(user.Theme);
    }
}