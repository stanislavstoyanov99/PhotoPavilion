namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HtmlAgilityPack;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Common;
    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Models.ViewModels.OrderProducts;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Services.Messaging;

    public class OrderProductsService : IOrderProductsService
    {
        private readonly IDeletableEntityRepository<OrderProduct> orderProductsRepository;
        private readonly IDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IEmailSender emailSender;

        public OrderProductsService(
            IDeletableEntityRepository<OrderProduct> orderProductsRepository,
            IDeletableEntityRepository<PhotoPavilionUser> usersRepository,
            IDeletableEntityRepository<Product> productsRepository,
            IShoppingCartsService shoppingCartsService,
            IEmailSender emailSender)
        {
            this.orderProductsRepository = orderProductsRepository;
            this.usersRepository = usersRepository;
            this.productsRepository = productsRepository;
            this.shoppingCartsService = shoppingCartsService;
            this.emailSender = emailSender;
        }

        public async Task<OrderProductDetailsViewModel> GetDetailsAsync(int id)
        {
            var orderProductDetailsViewModel = await this.orderProductsRepository
                .All()
                .To<OrderProductDetailsViewModel>()
                .FirstOrDefaultAsync(op => op.Id == id);

            if (orderProductDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.OrderProductNotFound, id));
            }

            return orderProductDetailsViewModel;
        }

        public IQueryable<OrderProductDetailsViewModel> GetAllAsQueryeable(string username)
        {
            var orderProductsViewModel = this.orderProductsRepository
                .All()
                .Where(op => op.User.UserName == username)
                .To<OrderProductDetailsViewModel>();

            return orderProductsViewModel;
        }

        public async Task BuyAllAsync(string userName, ShoppingCartProductViewModel[] shoppingCartProducts, string paymentMethod = "")
        {
            await this.BuyUsersTicketsAsync(userName, shoppingCartProducts, paymentMethod);
        }

        private async Task BuyUsersTicketsAsync(string userName, IEnumerable<ShoppingCartProductViewModel> shoppingCartProducts, string paymentMethod)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, userName));
            }

            var orderProductsIds = new List<int>();
            foreach (var shoppingCartProduct in shoppingCartProducts)
            {
                if (shoppingCartProduct.Quantity <= 0)
                {
                    throw new InvalidOperationException(ExceptionMessages.ZeroOrNegativeQuantity);
                }

                var product = await this.productsRepository.All()
                    .FirstAsync(p => p.Id == shoppingCartProduct.ProductId);

                var orderProductId = await this.SetOrderProductId(user.Id, product, shoppingCartProduct.Quantity);
                orderProductsIds.Add(orderProductId);
            }

            await this.shoppingCartsService.ClearShoppingCart(userName);

            var emailContent = await this.GenerateEmailContent(orderProductsIds, paymentMethod);
            await this.emailSender.SendEmailAsync(
                GlobalConstants.AdministratorEmail,
                GlobalConstants.SystemName,
                user.Email,
                ServicesDataConstants.BuyingEmailSubject,
                emailContent);
        }

        private async Task<int> SetOrderProductId(string userId, Product product, int quantity)
        {
            var orderProduct = new OrderProduct
            {
                UserId = userId,
                Product = product,
                Quantity = quantity,
                Status = OrderStatus.Pending,
                Date = DateTime.UtcNow,
            };

            await this.orderProductsRepository.AddAsync(orderProduct);
            await this.orderProductsRepository.SaveChangesAsync();

            return orderProduct.Id;
        }

        private async Task<string> GenerateEmailContent(IEnumerable<int> orderProductIds, string paymentMethod)
        {
            var ticketsInfo = new StringBuilder();
            var totalPrice = 0m;

            foreach (var orderProductId in orderProductIds)
            {
                var orderProduct = await this.orderProductsRepository
                    .All()
                    .FirstAsync(op => op.Id == orderProductId);

                var productName = orderProduct.Product.Name;
                var productQuantity = orderProduct.Quantity;
                var productPrice = orderProduct.Product.Price * orderProduct.Quantity;

                var ticketInfoHtml = string.Format(
                    ServicesDataConstants.OrderProductReceiptHtmlInfo,
                    orderProductId,
                    productName,
                    productQuantity,
                    productPrice);

                ticketsInfo.Append(ticketInfoHtml);
                totalPrice += productPrice;
            }

            var totalInfoHtml = string.Format(ServicesDataConstants.TotalReceiptHtmlInfo, totalPrice);

            var receiptPath = ServicesDataConstants.OrderProductsReceiptEmailHtmlPath;
            var doc = new HtmlDocument();
            doc.Load(receiptPath);

            paymentMethod = paymentMethod == GlobalConstants.OnlinePaymentMethod ?
                ServicesDataConstants.OnlinePaymentEmailString :
                ServicesDataConstants.CashPaymentEmailString;

            var content = doc.Text;
            content = content.Replace(ServicesDataConstants.TicketsInfoPlaceholder, ticketsInfo.ToString())
                .Replace(ServicesDataConstants.TotalReceiptInfoPlaceholder, totalInfoHtml)
                .Replace(ServicesDataConstants.PaymentMethodPlaceholder, paymentMethod);

            return content;
        }
    }
}
