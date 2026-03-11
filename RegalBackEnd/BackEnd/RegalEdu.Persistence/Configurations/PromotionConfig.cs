using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class PromotionConfig : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(x => x.Id);
           // builder.Property(x => x.Title).HasMaxLength(150).IsRequired();
        }
    }
}
