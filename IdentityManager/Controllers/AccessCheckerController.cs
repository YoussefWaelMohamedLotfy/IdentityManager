using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers;

[Authorize]
public class AccessCheckerController : Controller
{
    //Accessible by everyone, even if users are not logged in.
    [AllowAnonymous]
    public IActionResult AllAccess()
    {
        return View();
    }

    //Accessible by logged in users.
    [Authorize]
    public IActionResult AuthorizedAccess()
    {
        return View();
    }

    //Accessible by users who have user role
    [Authorize(Roles = "User")]
    public IActionResult UserAccess()
    {
        return View();
    }

    //Accessible by users who have user OR admin role
    [Authorize(Roles = "User,Admin")]
    public IActionResult UserORAdminAccess()
    {
        return View();
    }

    //Accessible by users who have user AND admin role
    [Authorize(Policy = "UserAndAdmin")]
    public IActionResult UserANDAdminAccess()
    {
        return View();
    }

    //Accessible by users who have admin role
    [Authorize(Policy = "Admin")]
    public IActionResult AdminAccess()
    {
        return View();
    }

    //Accessible by Admin users with a claim of create to be True
    [Authorize(Policy = "Admin_CreateAccess")]
    public IActionResult Admin_CreateAccess()
    {
        return View();
    }

    //Accessible by Admin user with claim of Create Edit and Delete (AND NOT OR)
    [Authorize(Policy = "Admin_Create_Edit_DeleteAccess")]
    public IActionResult Admin_Create_Edit_DeleteAccess()
    {
        return View();
    }

    //accessible by Admin user with create, edit and delete (AND NOT OR), OR if the user role is superAdmin
    [Authorize(Policy = "Admin_Create_Edit_DeleteAccess_OR_SuperAdmin")]
    public IActionResult Admin_Create_Edit_DeleteAccess_OR_SuperAdmin()
    {
        return View();
    }

    [Authorize(Policy = "AdminWithMoreThan1000Days")]
    public IActionResult OnlyBhrugen()
    {
        return View();
    }

    [Authorize(Policy = "FirstNameAuth")]
    public IActionResult FirstNameAuth()
    {
        return View();
    }
}
