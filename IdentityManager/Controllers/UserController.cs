using IdentityManager.Data;
using IdentityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace IdentityManager.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public IActionResult Index()
    {
        var userList = _db.ApplicationUsers.ToList();
        var userRole = _db.UserRoles.ToList();
        var roles = _db.Roles.ToList();

        foreach (var user in userList)
        {
            var role = userRole.FirstOrDefault(u => u.UserId == user.Id);

            if (role == null)
            {
                user.Role = "None";
            }
            else
            {
                user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
            }
        }

        return View(userList);
    }

    public IActionResult Edit(string userId)
    {
        var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

        if (objFromDb == null)
            return NotFound();

        var userRole = _db.UserRoles.ToList();
        var roles = _db.Roles.ToList();
        var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);

        if (role != null)
            objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;

        objFromDb.RoleList = _db.Roles.Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id
        });
        return View(objFromDb);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ApplicationUser user)
    {
        if (ModelState.IsValid)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);

            if (objFromDb == null)
                return NotFound();

            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);

            if (userRole != null)
            {
                var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                // Removing the old role
                await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);
            }

            // Add new role
            await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
            objFromDb.Name = user.Name;
            _db.SaveChanges();
            TempData[SD.Success] = "User has been edited successfully.";
            return RedirectToAction(nameof(Index));
        }

        user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Text = u.Name,
            Value = u.Id
        });
        return View(user);
    }

    [HttpPost]
    public IActionResult LockUnlock(string userId)
    {
        var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
        if (objFromDb == null)
        {
            return NotFound();
        }
        if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
        {
            // User is locked and will remain locked until lockout end time
            // Clicking on this action will unlock them
            objFromDb.LockoutEnd = DateTime.Now;
            TempData[SD.Success] = "User unlocked successfully.";
        }
        else
        {
            // User is not locked, and we want to lock the user
            objFromDb.LockoutEnd = DateTime.Now.AddDays(2);
            TempData[SD.Success] = "User locked successfully.";
        }

        _db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(string userId)
    {
        var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

        if (objFromDb == null)
            return NotFound();

        _db.ApplicationUsers.Remove(objFromDb);
        _db.SaveChanges();
        TempData[SD.Success] = "User deleted successfully.";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> ManageUserClaims(string userId)
    {
        IdentityUser user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        var existingUserClaims = await _userManager.GetClaimsAsync(user);

        var model = new UserClaimsViewModel()
        {
            UserId = userId
        };

        foreach (Claim claim in ClaimStore.claimsList)
        {
            UserClaim userClaim = new()
            {
                ClaimType = claim.Type
            };

            if (existingUserClaims.Any(c => c.Type == claim.Type))
                userClaim.IsSelected = true;

            model.Claims.Add(userClaim);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel userClaimsViewModel)
    {
        IdentityUser user = await _userManager.FindByIdAsync(userClaimsViewModel.UserId);

        if (user == null)
        {
            return NotFound();
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var result = await _userManager.RemoveClaimsAsync(user, claims);

        if (!result.Succeeded)
        {
            TempData[SD.Error] = "Error while removing claims";
            return View(userClaimsViewModel);
        }

        result = await _userManager.AddClaimsAsync(user, userClaimsViewModel.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString())));

        if (!result.Succeeded)
        {
            TempData[SD.Error] = "Error while adding claims";
            return View(userClaimsViewModel);
        }

        TempData[SD.Success] = "Claims updated successfully";
        return RedirectToAction(nameof(Index));
    }
}
