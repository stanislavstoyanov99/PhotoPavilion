namespace PhotoPavilion.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Common;
    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.ViewModels.Contacts;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Services.Messaging;

    public class ContactsService : IContactsService
    {
        private readonly IRepository<ContactFormEntry> userContactsRepository;
        private readonly IEmailSender emailSender;

        public ContactsService(
            IRepository<ContactFormEntry> contactsRepository,
            IEmailSender emailSender)
        {
            this.userContactsRepository = contactsRepository;
            this.emailSender = emailSender;
        }

        public async Task<IEnumerable<TModel>> GetAllUserEmailsAsync<TModel>()
        {
            var userEmails = await this.userContactsRepository
                .All()
                .To<TModel>()
                .ToListAsync();

            return userEmails;
        }

        public async Task SendContactToAdmin(ContactFormEntryViewModel contactFormViewModel)
        {
            var contactFormEntry = new ContactFormEntry
            {
                FirstName = contactFormViewModel.FirstName,
                LastName = contactFormViewModel.LastName,
                Email = contactFormViewModel.Email,
                Subject = contactFormViewModel.Subject,
                Content = contactFormViewModel.Content,
            };

            await this.userContactsRepository.AddAsync(contactFormEntry);
            await this.userContactsRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                contactFormViewModel.Email,
                string.Concat(contactFormViewModel.FirstName, " ", contactFormViewModel.LastName),
                GlobalConstants.SystemEmail,
                contactFormViewModel.Subject,
                contactFormViewModel.Content);
        }
    }
}
