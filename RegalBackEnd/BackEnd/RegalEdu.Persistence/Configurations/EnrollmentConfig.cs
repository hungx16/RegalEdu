using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Student)
                   .WithMany(x => x.Enrollments)
                   .HasForeignKey(x => x.StudentId);

            builder.HasOne(x => x.Class)
                   .WithMany(x => x.Enrollments)
                   .HasForeignKey(x => x.ClassId);
        }
    }
}
