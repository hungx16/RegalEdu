using RegalEdu.Domain.Models.DTO;
namespace RegalEdu.Domain.Models
{

    public class CompanyLearningRoadMapModel : BaseEntityModel
    {
        public Guid CompanyId { get; set; }
        public CompanyDto? Company { get; set; }
        public Guid LearningRoadMapId { get; set; }
        public LearningRoadMapDto? LearningRoadMap { get; set; }



    }
}
