using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class ClassConfig : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClassName).IsRequired();
            builder.HasOne(x => x.Teacher)
                   .WithMany(x => x.Classes)
                   .HasForeignKey(x => x.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict); // hoặc .Cascade tùy logic của bạn
        }
    }
}
