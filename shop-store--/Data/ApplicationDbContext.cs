using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shop_store.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;

namespace Shop_store.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        //unit TEST
        public ApplicationDbContext() : base(new DbContextOptions<ApplicationDbContext>())
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItemDto> ShoppingCartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.ShoppingCart)
                .WithOne(sc => sc.User)
                .HasForeignKey<ShoppingCart>(sc => sc.UserId);

       

            base.OnModelCreating(modelBuilder);
        }

    }
}
