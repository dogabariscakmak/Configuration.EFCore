using Microsoft.EntityFrameworkCore;

namespace Configuration.EFCore.TestCommon
{
    public class PersonDbContext : DbContext
    {
        public DbSet<PersonEntity> Persons { get; set; }

        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonEntity>();
            modelBuilder.ApplyConfiguration(new PersonEntityConfiguration());

            modelBuilder.BuildSettingEntityModel();

            base.OnModelCreating(modelBuilder);
        }
    }
}
