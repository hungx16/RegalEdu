namespace RegalEdu.Domain.Models;

public class EventReportPublicationItemModel
{
    public Guid CompanyEventReportId { get; set; }
    public string? CompanyEventReportCode { get; set; }
    public Guid CompanyEventId { get; set; }
    public string? CompanyEventName { get; set; }
    public DateTime EventDate { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal PublicationAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
