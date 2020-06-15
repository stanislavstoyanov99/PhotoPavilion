namespace PhotoPavilion.Services.Data.Common
{
    public static class ServicesDataConstants
    {
        public const string OrderProductReceiptHtmlInfo = @"<tr>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{0}</td>
                     <td align=""left"" width=""50%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{1}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{2}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">${3:F2}</td>
                     </tr>";

        public const string TotalReceiptHtmlInfo = @"<tr>
				  <td align=""left"" width=""50%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>Total</strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>${0:F2}</strong></td>
                </tr>";

        public const string OrderProductsReceiptEmailHtmlPath = "Views/Emails/product-orders-receipt.html";

        public const string OnlinePaymentEmailString = "Payed online";

        public const string CashPaymentEmailString = "Pay when you go to the shop";

        public const string TicketsInfoPlaceholder = "@productsInfo";

        public const string TotalReceiptInfoPlaceholder = "@totalInfo";

        public const string PaymentMethodPlaceholder = "@paymentMethod";

        public const string BuyingEmailSubject = "Buying confirmation";
    }
}
