using MathNet.Numerics.Distributions;
using MathNet.Numerics.RootFinding;
using Microsoft.EntityFrameworkCore;
using WeatherArchive.Models;

namespace WeatherArchive.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Weather> Weathers { get; set; }
    }
}
