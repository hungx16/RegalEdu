using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class TeacherConfig : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey (x => x.Id);

        }
    }
}
