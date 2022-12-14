using IdentityManager.Authorize;
using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    //options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    //options.SignIn.RequireConfirmedAccount = true;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserAndAdmin", policy => policy.RequireRole("Admin").RequireRole("User"));
    options.AddPolicy("Admin_CreateAccess", policy => policy.RequireRole("Admin").RequireClaim("create", "True"));
    options.AddPolicy("Admin_Create_Edit_DeleteAccess", policy => policy.RequireRole("Admin").RequireClaim("create", "True")
        .RequireClaim("edit", "True")
        .RequireClaim("Delete", "True"));

    options.AddPolicy("Admin_Create_Edit_DeleteAccess_OR_SuperAdmin", policy => policy.RequireAssertion(context => AuthorizeAdminWithClaimsOrSuperAdmin(context)));
    options.AddPolicy("OnlySuperAdminChecker", policy => policy.Requirements.Add(new OnlySuperAdminChecker()));
    options.AddPolicy("AdminWithMoreThan1000Days", policy => policy.Requirements.Add(new AdminWithMoreThan1000DaysRequirement(1000)));
    options.AddPolicy("FirstNameAuth", policy => policy.Requirements.Add(new FirstNameAuthRequirement("billy")));
});

//builder.Services.ConfigureApplicationCookie(opt =>
//{
//    opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
//});

builder.Services.AddTransient<IEmailSender, SendGridEmailService>();
builder.Services.Configure<SendGridOptions>(options =>
{
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
    options.SenderEmail = builder.Configuration["SendGrid:SenderEmail"];
    options.SenderName = builder.Configuration["SendGrid:SenderName"];
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "2797980420437778";
    options.AppSecret = "abe6f05cc42cb58fef1e689b54a04011";
});

builder.Services.AddScoped<IAuthorizationHandler, AdminWithOver1000DaysHandler>();
builder.Services.AddScoped<IAuthorizationHandler, FirstNameAuthHandler>();
builder.Services.AddScoped<INumberOfDaysForAccount, NumberOfDaysForAccount>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

bool AuthorizeAdminWithClaimsOrSuperAdmin(AuthorizationHandlerContext context)
    => (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Create" && c.Value == "True")
                && context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True")
                && context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")
            ) || context.User.IsInRole("SuperAdmin");