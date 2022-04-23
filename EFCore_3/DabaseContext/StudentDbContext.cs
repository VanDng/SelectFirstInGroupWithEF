using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore_3
{
    class StudentDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        // Pay attension here, the data type of the two StudentNoID collections is different.
        public DbQuery<StudentNoID1> StudentNoID1s { get; set; } // There will be a warning that DbQuery is obsoleted.
        public DbSet<StudentNoID2> StudentNoID2s { get; set; }

        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }

        public StudentDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = CommonConfig.ConnectionString.Get();
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentNoID2>().HasNoKey();
        }
    }
}
