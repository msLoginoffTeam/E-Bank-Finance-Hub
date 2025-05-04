using System.ComponentModel.DataAnnotations;

namespace UserPreferences.Models;

public class UserSettings
{
    [Key]
    public Guid UserId { get; set; }
    public ThemeEnum Theme { get; set; }

    
    public UserSettings(Guid userId, ThemeEnum theme)
    {
        UserId = userId;
        Theme = theme;
    }
}