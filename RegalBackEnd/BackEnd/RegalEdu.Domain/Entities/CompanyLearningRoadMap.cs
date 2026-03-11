using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("CompanyLearningRoadMap")]
    public class CompanyLearningRoadMap : BaseEntity
    {
        public Guid CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        public Company? Company { get; set; }
        public Guid LearningRoadMapId { get; set; }
        [ForeignKey ("LearningRoadMapId")]
        public LearningRoadMap? LearningRoadMap { get; set; }


    }
}
