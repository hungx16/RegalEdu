
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class WorkingTimeConfigurationCompanyModel : BaseEntityModel
    {
        public Guid CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        public CompanyModel? Company { get; set; } = null!;
        public Guid WorkingTimeConfigurationId { get; set; }
        [ForeignKey ("WorkingTimeConfigurationId")]
        public WorkingTimeConfigurationModel? WorkingTimeConfiguration { get; set; } = null!;

    }
}
