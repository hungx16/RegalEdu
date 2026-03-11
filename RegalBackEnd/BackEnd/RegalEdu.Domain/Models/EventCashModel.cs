using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table("EventCash")]
public class EventCashModel : BaseEntityModel
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
