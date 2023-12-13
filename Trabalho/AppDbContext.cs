using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Trabalho
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Client> Clients => Set<Client>();
        public virtual DbSet<Game> Games => Set<Game>();
        public virtual DbSet<Rental> Rentals => Set<Rental>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rental>()
            .HasKey(r => new { r.ClientId, r.GameId });

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.ClientId);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Rentals)
                .HasForeignKey(r => r.GameId);
        }
    }
}
