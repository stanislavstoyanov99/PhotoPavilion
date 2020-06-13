namespace PhotoPavilion.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    public class StarRating : BaseDeletableModel<int>
    {
        public int Rate { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public string UserId { get; set; }

        public PhotoPavilionUser User { get; set; }

        public DateTime NextVoteDate { get; set; }
    }
}
