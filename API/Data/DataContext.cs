using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}