using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Configuration.EFCore.TestCommon
{
    public class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
    {
        public PersonEntityConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<PersonEntity> builder)
        {
            #region Configuration
            builder.ToTable("Persons");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            #endregion
        }
    }
}
