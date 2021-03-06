﻿// ReSharper disable VirtualMemberCallInConstructor
namespace PhotoPavilion.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    using PhotoPavilion.Data.Common;
    using PhotoPavilion.Data.Common.Models;
    using PhotoPavilion.Data.Models.Enumerations;

    public class PhotoPavilionUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public PhotoPavilionUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();

            this.OrderProducts = new HashSet<OrderProduct>();
            this.Comments = new HashSet<Comment>();
            this.Reviews = new HashSet<Review>();
            this.Ratings = new HashSet<StarRating>();
        }

        [Required]
        [MaxLength(DataValidation.FullNameMaxLength)]
        public string FullName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public int ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<StarRating> Ratings { get; set; }
    }
}
