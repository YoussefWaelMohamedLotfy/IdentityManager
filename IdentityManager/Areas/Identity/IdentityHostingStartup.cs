[assembly: HostingStartup(typeof(IdentityManager.Areas.Identity.IdentityHostingStartup))]
namespace IdentityManager.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}