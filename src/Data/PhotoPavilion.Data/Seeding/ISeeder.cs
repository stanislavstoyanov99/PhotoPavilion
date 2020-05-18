namespace PhotoPavilion.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task SeedAsync(PhotoPavilionDbContext dbContext, IServiceProvider serviceProvider);
    }
}
