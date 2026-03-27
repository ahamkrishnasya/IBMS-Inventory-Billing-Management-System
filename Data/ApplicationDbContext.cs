using IBMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        #region DB SETS
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        #endregion

        #region Model Builder
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //customer
            builder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.ContactNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(c => c.Email)
                    .IsRequired();

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            //supplier
            builder.Entity<Supplier>(entity =>
            {
                entity.Property(s => s.SupplierName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.ContactNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(s => s.Email)
                    .IsRequired();

                entity.Property(s => s.Address)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            //product
            builder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ProductName).IsRequired();
                entity.Property(p => p.Category).IsRequired();

                entity.Property(p => p.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(p => p.Quantity).IsRequired();

                entity.Property(p => p.TaxRate)
                    .IsRequired()
                    .HasPrecision(5, 2);
            });

            //stock
            builder.Entity<Stock>(entity =>
            {
                entity.HasOne(s => s.Product)
                    .WithMany()
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.CurrentStock).IsRequired();
                entity.Property(s => s.ReorderLevel).IsRequired();
            });

            //purchase
            builder.Entity<Purchase>(entity =>
            {
                entity.HasOne(p => p.Supplier)
                    .WithMany()
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Quantity).IsRequired();

                entity.Property(p => p.TotalAmount)
                    .HasPrecision(18, 2);
            });

            //sale
            builder.Entity<Sale>(entity =>
            {
                entity.HasOne(s => s.Customer)
                    .WithMany()
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Product)
                    .WithMany()
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.Quantity).IsRequired();

                entity.Property(s => s.TotalAmount).HasPrecision(18, 2);
                entity.Property(s => s.TaxAmount).HasPrecision(18, 2);
                entity.Property(s => s.NetAmount).HasPrecision(18, 2);
            });

            //invoice
            builder.Entity<Invoice>(entity =>
            {
                entity.HasOne(i => i.Sale)
                    .WithMany()
                    .HasForeignKey(i => i.SaleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.InvoiceDate).IsRequired();

                entity.Property(i => i.PaymentMode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.InvoiceTotal)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.HasIndex(i => i.SaleId)
                    .IsUnique();
            });
        }
        #endregion
    }
}