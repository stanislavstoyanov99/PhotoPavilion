namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using PhotoPavilion.Models.ViewModels.Contacts;

    public interface IContactsService
    {
        Task SendContactToAdmin(ContactFormEntryViewModel contactFormEntryViewModel);
    }
}
