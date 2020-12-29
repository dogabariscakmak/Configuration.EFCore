using Configuration.EFCore.TestCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Configuration.EFCore.IntegrationTest.Helpers
{
    public class PersonDbDesignContext : IDesignTimeDbContextFactory<PersonDbContext>
    {
        public PersonDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PersonDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433 ;User=sa;Password=P@ssw0rd123", b => b.MigrationsAssembly("Configuration.EFCore.IntegrationTest"));

            return new PersonDbContext(optionsBuilder.Options);
        }
    }
}
