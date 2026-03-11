using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Persistence.Configurations
{

    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
