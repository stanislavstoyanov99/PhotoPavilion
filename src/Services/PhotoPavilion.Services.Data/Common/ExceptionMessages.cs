namespace PhotoPavilion.Services.Data.Common
{
    public static class ExceptionMessages
    {
        public const string ProductAlreadyExists = "Product with name {0} already exists";

        public const string ProductNotFound = "Product with id {0} is not found.";

        public const string BrandAlreadyExists = "Brand with name {0} already exists";

        public const string BrandNotFound = "Brand with id {0} is not found.";

        public const string CategoryAlreadyExists = "Category with name {0} already exists";

        public const string CategoryNotFound = "Category with id {0} is not found.";

        public const string PrivacyAlreadyExists = "Privacy with page content {0} already exists";

        public const string PrivacyNotFound = "Privacy with id {0} is not found.";

        public const string PrivacyViewModelNotFound = "Privacy view model is not found.";

        public const string AlreadySentVote = "You cannot vote twice in the same day. To vote come back again tomorrow at";

        public const string AuthenticatedErrorMessage = "Please, login in order to vote.";

        public const string ProductCommentAlreadyExists = "Product comment with product id {0} and content {1} already exists";

        public const string NullReferenceShoppingCart = "User with id {0} and username {1} does not have a shopping cart.";

        public const string NullReferenceUsername = "User with username {0} is not found.";

        public const string ZeroOrNegativeQuantity = "Quantity cannot be negative or zero";

        public const string NullReferenceGuestShoppingCartProductId = "Session does not contain shopping cart product with id {0}.";

        public const string NullReferenceShoppingCartProductId = "Shopping cart product with id {0} not found.";

        public const string OrderProductNotFound = "Order product with id {0} is not found.";
    }
}
