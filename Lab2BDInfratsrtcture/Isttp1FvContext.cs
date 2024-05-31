using Microsoft.EntityFrameworkCore;
using Lab2BDDomain.Model;

namespace Lab2BDInfrastructure
{
    public class Isttp1FvContext : DbContext
    {
        public Isttp1FvContext(DbContextOptions<Isttp1FvContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<FactoryProduct> FactoryProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ReqProduct> ReqProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientName).HasColumnType("text");
                entity.Property(e => e.Contacts).HasColumnType("text");

                // Configure cascade delete for Orders related to Client
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Client)
                      .HasForeignKey(o => o.ClientId)
                      .OnDelete(DeleteBehavior.Cascade); // Ensures orders are deleted when a client is deleted
            });

            modelBuilder.Entity<Factory>(entity =>
            {
                entity.Property(e => e.Adress).HasColumnType("text");
                entity.Property(e => e.FactoryName).HasColumnType("text");
                entity.Property(e => e.Maintenance).HasColumnType("text");
            });

            modelBuilder.Entity<FactoryProduct>(entity =>
            {
                entity.HasOne(d => d.Factory).WithMany(p => p.FactoryProducts)
                    .HasForeignKey(d => d.FactoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FactoryProducts_Factories");

                entity.HasOne(d => d.Product).WithMany(p => p.FactoryProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FactoryProducts_Products");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade) // Ensures ReqProducts are deleted when an order is deleted
                    .HasConstraintName("FK_Orders_Clients");

                entity.Property(e => e.OrderStatus).HasColumnType("int");
                entity.Property(e => e.OrderDate).HasColumnType("date");
                entity.Property(e => e.ApprComplDate).HasColumnType("date");
                entity.Property(e => e.OrderPrice).HasColumnType("int");

                // Configure cascade delete for ReqProducts related to Order
                entity.HasMany(e => e.ReqProducts)
                      .WithOne(rp => rp.Order)
                      .HasForeignKey(rp => rp.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductDescription).HasColumnType("text");
                entity.Property(e => e.ProductName).HasColumnType("text");
            });

            modelBuilder.Entity<ReqProduct>(entity =>
            {
                entity.HasOne(d => d.Product).WithMany(p => p.ReqProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReqProducts_Products");

                entity.HasOne(d => d.Order).WithMany(p => p.ReqProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade) // Ensure cascading delete
                    .HasConstraintName("FK_ReqProducts_Orders");
            });
        }
    }
}
