using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcIntegrationServerApp.Models.Map
{
    class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            ToTable("Customer");

            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.ContextIdentifier)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Name)
                .HasMaxLength(255);

            this.Property(t => t.SystemUserToken)
                .HasMaxLength(255);

            this.Property(t => t.NetServerUrl)
                .HasMaxLength(255);

            this.HasOptional(t => t.RegisteredBy)
                .WithMany()
                .HasForeignKey(t => t.RegisteredId)
                .WillCascadeOnDelete(false);

            this.HasOptional(t => t.LastUsedBy)
                .WithMany()
                .HasForeignKey(t => t.LastUsedById)
                .WillCascadeOnDelete(false);
        }
    }
}
