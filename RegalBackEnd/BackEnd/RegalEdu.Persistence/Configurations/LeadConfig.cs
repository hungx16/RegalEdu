using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Persistence.Configurations
{
    public class LeadConfig : IEntityTypeConfiguration<Lead>
    {
        public void Configure(EntityTypeBuilder<Lead> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.Status).HasMaxLength(50);
        }
    }
}
