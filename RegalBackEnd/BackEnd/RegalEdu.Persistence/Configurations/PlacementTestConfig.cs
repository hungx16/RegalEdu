using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class PlacementTestConfig : IEntityTypeConfiguration<PlacementTest>
    {
        public void Configure(EntityTypeBuilder<PlacementTest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Student)
                   .WithMany()
                   .HasForeignKey(x => x.StudentId);
        }
    }
}
