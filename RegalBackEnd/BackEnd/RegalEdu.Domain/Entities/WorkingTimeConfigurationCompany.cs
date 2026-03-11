using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("WorkingTimeConfigurationCompany")]
    public class WorkingTimeConfigurationCompany : BaseEntity
    {
        public Guid CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        public Company? Company { get; set; } = null!;
        public Guid WorkingTimeConfigurationId { get; set; }
        [ForeignKey ("WorkingTimeConfigurationId")]
        public WorkingTimeConfiguration? WorkingTimeConfiguration { get; set; } = null!;

    }
}
