using Microsoft.EntityFrameworkCore;

namespace dotNetAPI.Entity
{
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext()
        {
        }

        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Company { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CarStatus> CarStatus { get; set; }
        public DbSet<VehicleQuote> VehicleQuote { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("AppDb");
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
