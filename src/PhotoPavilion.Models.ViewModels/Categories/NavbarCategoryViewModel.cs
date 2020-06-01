namespace PhotoPavilion.Models.ViewModels.Categories
{
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    public class NavbarCategoryViewModel : IMapFrom<Category>
    {
        public string Name { get; set; }
    }
}
