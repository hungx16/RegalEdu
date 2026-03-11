using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("EventPublication")]
public class EventPublication : BaseEntity
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
