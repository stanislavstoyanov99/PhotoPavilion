namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.ViewModels.Contacts;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Services.Messaging;

    using Xunit;

    public class ContactsServiceTests : IDisposable, IClassFixture<Configuration>
    {
        private readonly IEmailSender emailSender;
        private readonly IContactsService contactsService;

        private EfRepository<ContactFormEntry> userContactsRepository;
        private SqliteConnection connection;

        private ContactFormEntry firstUserContactFormEntry;

        public ContactsServiceTests(Configuration configuration)
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.emailSender = new SendGridEmailSender(configuration.ConfigurationRoot["SendGrid:ApiKey"]);
            this.contactsService = new ContactsService(this.userContactsRepository, this.emailSender);
        }

        [Fact]
        public async Task CheckIfSendContactToAdminWorksCorrectly()
        {
            var model = new ContactFormEntryViewModel
            {
                FirstName = "Peter",
                LastName = "Kirov",
                Email = "peter.kirov@abv.bg",
                Subject = "Question about cinema news",
                Content = "Sample content about cinema news",
            };

            await this.contactsService.SendContactToAdmin(model);
            var count = await this.userContactsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        public void Dispose()
        {
            this.connection.Close();
            this.connection.Dispose();
        }

        private void InitializeDatabaseAndRepositories()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<PhotoPavilionDbContext>().UseSqlite(this.connection);
            var dbContext = new PhotoPavilionDbContext(options.Options);

            dbContext.Database.EnsureCreated();

            this.userContactsRepository = new EfRepository<ContactFormEntry>(dbContext);
        }

        private void InitializeFields()
        {
            this.firstUserContactFormEntry = new ContactFormEntry
            {
                FirstName = "Stanislav",
                LastName = "Stoyanov",
                Email = "slavkata_99@abv.bg",
                Subject = "I have a question",
                Content = "I have to ask you something connected with the system.",
            };
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("PhotoPavilion.Models.ViewModels"));
    }
}
