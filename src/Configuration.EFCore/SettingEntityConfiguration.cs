using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Configuration.EFCore
{
    public class SettingEntityConfiguration : IEntityTypeConfiguration<SettingEntity>
    {
        public SettingEntityConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<SettingEntity> builder)
        {
            #region Configuration
            builder.ToTable("Settings");
            builder.HasKey(x => new { x.Key, x.Environment, x.Application });
            #endregion
        }
    }
}
