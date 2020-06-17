namespace PhotoPavilion.Web
{
    using System;
    using System.Reflection;

    using CloudinaryDotNet;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Common;
    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Data.Seeding;
    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Services.Data;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Services.Messaging;
    using PhotoPavilion.Web.Common;
    using PhotoPavilion.Web.Middlewares;

    using Stripe;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PhotoPavilionDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<PhotoPavilionUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<PhotoPavilionDbContext>();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = new TimeSpan(0, 4, 0, 0);
            });

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews(
                options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddRazorPages();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(
                serviceProvider => new SendGridEmailSender(this.configuration["SendGrid:ApiKey"]));
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<IBrandsService, BrandsService>();
            services.AddTransient<IPrivacyService, PrivacyService>();
            services.AddTransient<IRatingsService, RatingsService>();
            services.AddTransient<IProductCommentsService, ProductCommentsService>();
            services.AddTransient<IShoppingCartsService, ShoppingCartsService>();
            services.AddTransient<IOrderProductsService, OrderProductsService>();
            services.AddTransient<IAboutService, AboutService>();
            services.AddTransient<IProductReviewsService, ProductReviewsService>();
            services.AddTransient<IContactsService, ContactsService>();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = this.configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = this.configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.Fields.Add("name");
                });

            var account = new CloudinaryDotNet.Account(
                this.configuration["Cloudinary:AppName"],
                this.configuration["Cloudinary:AppKey"],
                this.configuration["Cloudinary:AppSecret"]);

            Cloudinary cloudinary = new Cloudinary(account);

            services.AddSingleton(cloudinary);

            // Stripe
            services.Configure<StripeSettings>(this.configuration.GetSection("Stripe"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<PhotoPavilionDbContext>();

                dbContext.Database.Migrate();

                new ApplicationDbContextSeeder()
                    .SeedAsync(dbContext, serviceScope.ServiceProvider)
                    .GetAwaiter()
                    .GetResult();
            }

            StripeConfiguration.ApiKey = this.configuration.GetSection("Stripe")["SecretKey"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseStatusCodePagesWithRedirects("/Home/HttpError?statusCode={0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseSession();
            app.UseAuthorization();
            app.UseAdminMiddleware();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute(
                            "categoryName",
                            "category/{name:minlength(3)}",
                            new { controller = "Categories", action = "ByName" });
                        endpoints.MapRazorPages();
                    });
        }
    }
}
