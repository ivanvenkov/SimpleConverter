using JConverter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JConverter.Data
{
    public class ConverterDbContext : DbContext
    {
        public ConverterDbContext(DbContextOptions<ConverterDbContext> options) : base(options)
        {
        }

        public DbSet<FileEntity> Files { get; set; }

    }
}