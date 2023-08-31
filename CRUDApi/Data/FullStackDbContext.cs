using CRUDApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUDApi.Data
{
    public class FullStackDbContext : DbContext
    {
        public FullStackDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
