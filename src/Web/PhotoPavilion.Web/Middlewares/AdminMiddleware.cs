namespace PhotoPavilion.Web.Middlewares
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using PhotoPavilion.Common;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;

    public class AdminMiddleware
    {
        private readonly RequestDelegate next;

        public AdminMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<PhotoPavilionUser> userManager)
        {
            await this.SeedUserInRoles(userManager);
            await this.next(context);
        }

        private async Task SeedUserInRoles(UserManager<PhotoPavilionUser> userManager)
        {
            if (!userManager.Users.Any(x => x.UserName == GlobalConstants.AdministratorUsername))
            {
                var user = new PhotoPavilionUser
                {
                    UserName = GlobalConstants.AdministratorUsername,
                    Email = GlobalConstants.AdministratorEmail,
                    FullName = GlobalConstants.AdministratorFullName,
                    Gender = Gender.Male,
                    ShoppingCart = new ShoppingCart(),
                };

                var result = await userManager.CreateAsync(user, GlobalConstants.AdministratorPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                }
                else
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
