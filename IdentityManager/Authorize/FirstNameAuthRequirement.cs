using Microsoft.AspNetCore.Authorization;

namespace IdentityManager.Authorize;

public class FirstNameAuthRequirement : IAuthorizationRequirement
{
    public string Name { get; set; }

    public FirstNameAuthRequirement(string name)
    {
        Name = name;
    }
}
