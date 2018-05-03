using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcIntegrationServerApp.Models.Map
{
    class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("User");

            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.UserPrincipalName)
                .HasMaxLength(255);

            this.Property(t => t.Email)
                .HasMaxLength(255);

            this.HasRequired(t => t.Customer)
                .WithMany(t => t.Users)
                .HasForeignKey(t => t.CustomerId)
                .WillCascadeOnDelete(false);


        }
    }
}
