using IdentityManager.Data;

namespace IdentityManager.Authorize;

public class NumberOfDaysForAccount : INumberOfDaysForAccount
{
    private readonly ApplicationDbContext _db;

    public NumberOfDaysForAccount(ApplicationDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public int Get(string userId)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

        if (user != null && user.DateCreated != DateTime.MinValue)
            return (DateTime.Today - user.DateCreated).Days;

        return 0;
    }
}
