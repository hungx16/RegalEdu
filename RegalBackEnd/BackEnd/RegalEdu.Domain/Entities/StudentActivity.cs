
namespace RegalEdu.Domain.Entities;

public class StudentActivity : BaseEntity
{
    public Guid? StudentId { get; set; }
    public virtual Student? Student { get; set; }
    public Guid? EmployeeId { get; set; } //Người thực hiện
    public virtual Employee? Employee { get; set; }
    public string? Type { get; set; } //Có các giá trị: 0-Gọi điện/ 1-Tin nhắn/ 2-Thư điện tử/ 3-Sự kiện 
    public DateTime? ActivityDate{ get; set; } //Ngày tương tác
    public string? Results { get; set; }
    public string? NextAction { get; set; } //hành động tiếp theo
    public string? Content { get; set; }//nội dung tương tác
    public string? CallLogURL { get; set; } //Lưu URL CallLog theo từng cuộc gọi. 
    
    public string? CallId { get; set; } //mã cuộc gọi
    public string? UserID { get; set; } //mã nhân viên gọi
    public string? StatusCode { get; set; } //mã trạng thái gọi
    public string? ReasonFailed { get; set; } //lý do thất bại
}
