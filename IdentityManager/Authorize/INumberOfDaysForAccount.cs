namespace IdentityManager.Authorize
{
    public interface INumberOfDaysForAccount
    {
        int Get(string userId);
    }
}
