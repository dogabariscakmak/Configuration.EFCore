using Microsoft.EntityFrameworkCore;

namespace Configuration.EFCore
{
    public static class ModelBuilderExtensions
    {
        public static void BuildSettingEntityModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SettingEntity>();

            modelBuilder.ApplyConfiguration(new SettingEntityConfiguration());
        }
    }
}
