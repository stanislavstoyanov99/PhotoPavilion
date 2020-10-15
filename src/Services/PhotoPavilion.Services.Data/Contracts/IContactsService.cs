namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.ViewModels.Contacts;

    public interface IContactsService
    {
        Task SendContactToAdmin(ContactFormEntryViewModel contactFormEntryViewModel);
    }
}
