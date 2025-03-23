using HotelReservationsManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationsManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
<<<<<<< HEAD
=======
        public virtual DbSet<Client> Clients { get; set; }
        //public virtual DbSet<Room> Rooms { get; set; }
        //public virtual DbSet<Reservation> Reservations { get; set; }
        //public virtual DbSet<ClientHistory> ClientHistories { get; set; }
>>>>>>> 86b62af0151b9cb2c0fcedebffbdd84d11f40a92
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext()
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
