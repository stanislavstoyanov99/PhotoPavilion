namespace PhotoPavilion.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PhotoPavilion.Models.ViewModels.About;
    using PhotoPavilion.Services.Data.Contracts;

    public class AboutController : Controller
    {
        private readonly IAboutService aboutService;

        public AboutController(IAboutService aboutService)
        {
            this.aboutService = aboutService;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await this.aboutService.GetAllFaqsAsync<FaqDetailsViewModel>();

            return this.View(faqs);
        }
    }
}
