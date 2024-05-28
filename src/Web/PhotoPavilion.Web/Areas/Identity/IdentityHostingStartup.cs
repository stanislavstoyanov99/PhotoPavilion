using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(PhotoPavilion.Web.Areas.Identity.IdentityHostingStartup))]

namespace PhotoPavilion.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}