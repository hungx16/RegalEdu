using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table("EventPublication")]
public class EventPublicationModel : BaseEntityModel
{
    public Guid? CompanyEventId { get; set; }
    public Guid? CompanyEventReportId { get; set; }
    public Guid? ItemId { get; set; } // formerly publicationId
    public virtual Item? Item { get; set; }
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal PublicationAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal TotalAmount { get; set; }
}
