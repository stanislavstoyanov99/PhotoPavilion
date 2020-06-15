namespace PhotoPavilion.Data.Models
{
    using System.Collections.Generic;

    using PhotoPavilion.Data.Common.Models;

    public class ShoppingCart : BaseDeletableModel<int>
    {
        public ShoppingCart()
        {
            this.ShoppingCartProducts = new HashSet<ShoppingCartProduct>();
        }

        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }
    }
}
