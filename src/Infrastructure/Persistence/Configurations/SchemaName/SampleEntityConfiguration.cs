using Domain.Entities.SchemaName;

namespace Infrastructure.Persistence.Configurations.SchemaName;

public class SampleEntityConfiguration : IEntityTypeConfiguration<SampleEntity>
{
    public void Configure(EntityTypeBuilder<SampleEntity> builder)
    {
        builder.ToTable("SampleEntity", "SchemaName");
        builder.HasKey(x => x.Id);
    }
}