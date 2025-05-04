using Microsoft.AspNetCore.Mvc;
using UserPreferences.Models;

namespace UserPreferences.Services;

public class AccountsService
{
    private readonly AppDbContext _db;
    public AccountsService(AppDbContext db) => _db = db;

    public ActionResult AddAccountRecord(Guid userId, Guid accountId)
    {
        var existingRecord = _db.HiddenAccounts.Find(accountId);
        if (existingRecord != null)
        {
            return new BadRequestResult(); 
        }
        
        var user = _db.UserSettings.FirstOrDefault(u => u.UserId == userId);

        if (user == null)
        {
            return new NotFoundObjectResult(userId);
        }
        
        var newHiddenAccountRecord = new HiddenAccount(accountId)
        {
            User = user
        };

        _db.Add(newHiddenAccountRecord);
        _db.SaveChanges();
        return new OkResult();
    }

    public ActionResult DeleteAccountRecord(Guid accountId)
    {
        var existingRecord = _db.HiddenAccounts.Find(accountId);
        
        if (existingRecord == null)
        {
            return new NotFoundObjectResult(accountId); 
        }
        
        _db.Remove(existingRecord);
        _db.SaveChanges();
        return new OkResult();
    }

    public ActionResult<bool> IsHiddenAccountR(Guid accountId)
    {
        var existingRecord = _db.HiddenAccounts.Find(accountId);

        return existingRecord != null;
    }
}