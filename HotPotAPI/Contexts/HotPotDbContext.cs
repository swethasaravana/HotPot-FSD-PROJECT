using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HotPotAPI.Contexts
{
    public class HotPotDbContext : DbContext
    {
        public HotPotDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }  
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantManager> RestaurantManagers { get; set; }
        public DbSet<DeliveryPartner> DeliveryPartners { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer → User relationship
            modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.Email)
            .HasPrincipalKey<User>(u => u.Username)
            .OnDelete(DeleteBehavior.Cascade);

            // Customer → Cart relationship
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(cu => cu.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Admin relationship
            modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne(u => u.Admin)
            .HasForeignKey<Admin>(a => a.Email)
            .HasPrincipalKey<User>(u => u.Username)
            .OnDelete(DeleteBehavior.Cascade);

            // RestaurantManager relationship
            modelBuilder.Entity<RestaurantManager>()
            .HasOne(rm => rm.User)
            .WithOne(u => u.RestaurantManager)
            .HasForeignKey<RestaurantManager>(rm => rm.Email)
            .HasPrincipalKey<User>(u => u.Username)
            .OnDelete(DeleteBehavior.Cascade);

            // DeliveryPartner relationship
            modelBuilder.Entity<DeliveryPartner>()
            .HasOne(dp => dp.User)
            .WithOne(u => u.DeliveryPartner)
            .HasForeignKey<DeliveryPartner>(dp => dp.Username)
            .HasPrincipalKey<User>(u => u.Username)
            .OnDelete(DeleteBehavior.Cascade);

            // MenuItem → Cuisine (many-to-one)
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Cuisine)
                .WithMany()
                .HasForeignKey(m => m.CuisineId)
                .OnDelete(DeleteBehavior.Restrict);

            // MenuItem → MealType (many-to-one)
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.MealType)
                .WithMany()
                .HasForeignKey(m => m.MealTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // MenuItem → Restaurant (many-to-one)
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Restaurant)
                .WithMany()
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart → Customer (one-to-one relationship)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(cu => cu.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem → Cart (many-to-one relationship)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem → MenuItem (many-to-one relationship)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.MenuItem)
                .WithMany()
                .HasForeignKey(ci => ci.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem → PriceAtPurchase (decimal type)
            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.PriceAtPurchase)
                .HasColumnType("decimal(10, 2)");

            // CartItem → Quantity default value
            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Quantity)
                .HasDefaultValue(1);

            // Customer → CustomerAddress relationship (One-to-Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Addresses) 
                .WithOne(ca => ca.Customer) // Each CustomerAddress is related to one Customer
                .HasForeignKey(ca => ca.CustomerId) 
                .OnDelete(DeleteBehavior.Cascade); 

            // Configure the properties for CustomerAddress
            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.Label)
                .HasMaxLength(255)
                .IsRequired(); 

            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.Street)
                .HasMaxLength(255)
                .IsRequired(); 

            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.City)
                .HasMaxLength(100)
                .IsRequired(); 

            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.Pincode)
                .HasMaxLength(10)
                .IsRequired();

            // Order → Customer (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → CustomerAddress (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.CustomerAddress)
                .WithMany()
                .HasForeignKey(o => o.CustomerAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → OrderStatus (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → PaymentMethod (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentMethod)
                .WithMany()
                .HasForeignKey(o => o.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → PaymentStatus (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentStatus)
                .WithMany()
                .HasForeignKey(o => o.PaymentStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → OrderItems (one-to-many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → MenuItem (many-to-one)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem → Price (decimal type)
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");

            // Order → TotalAmount (decimal type) — Added this
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");


        }
    }
}
