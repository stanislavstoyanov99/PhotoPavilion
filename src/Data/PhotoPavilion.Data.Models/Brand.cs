namespace PhotoPavilion.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Brand;

    public class Brand : BaseDeletableModel<int>
    {
        public Brand()
        {
            this.Products = new HashSet<Product>();
        }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
