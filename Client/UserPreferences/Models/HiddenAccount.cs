using System.ComponentModel.DataAnnotations;

namespace UserPreferences.Models;

public class HiddenAccount
{
    [Key]
    public Guid Id { get; set; }
    public UserSettings User { get; set; }
    
    private HiddenAccount() {}
    public HiddenAccount(Guid id)
    {
        Id = id;
    }
}