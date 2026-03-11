using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("EventCash")]
public class EventCash : BaseEntity
{
    public Guid? CompanyEventId { get; set; }
    public Guid? CompanyEventReportId { get; set; }

    public string CashName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal Amount { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal TotalAmount { get; set; }
}
