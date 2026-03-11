
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class Customer : BaseEntity
{
    //thông tin khách hàng - học viên
    public string CustomerCode { get; set; } = string.Empty;//mã khách hàng tự động tăng
    public string FullName { get; set; } = string.Empty; //Họ tên 
    public int? BirthYear { get; set; }//năm sinh
    public string? Email { get; set; } //email có hoặc không
    public string? ParentPhone{ get; set; }//số điện thoại phụ huynh
    public string? ParentName { get; set; } //tên phụ huynh
    public string? InformationChannel { get; set; } //kênh thông tin
    public string? LeadSource { get; set; } //có các giá trị: Website /Social /Event /Zalo /Facebook /Tiktok /Support/Khác
    public string? InterestedProgram { get; set; } //chương trình học quan tâm
    //khai báo loại khách hàng
    public CustomerType? CustomerType { get; set; } //loại khách hàng tiềm năng/tìm hiểu, tiếp cận
    //các trường thông tin cá nhân thêm
    public string? Gender { get; set; } //giới tính nam, nữ, khác
    public DateTime? BirthDate { get; set; }//ngày sinh
    public string? Address { get; set; } //địa chỉ 
    //nhân viên tư vấn phụ trách - liên kết với bảng nhân viên mỗi học sinh có một nhân viên tư vấn
    public Guid? EmployeeId { get; set; }
    [ForeignKey ("EmployeeId")]
    public virtual Employee? Employee { get; set; } //nhân viên tư vấn
    public Guid? ManagerId { get; set; }//quản lý trực tiếp của nhân viên tư vấn lấy từ bảng Employee
    public Guid? AffiliateId { get; set; }//Cộng tác viên giới thiệu lấy từ bảng Employee

    //chi nhánh hiện tại của học sinh - liên kêt với bảng chi nhánh mỗi học sinh thuộc một chi nhánh
    public Guid? CompanyId { get; set; }
    

    [ForeignKey ("CompanyId")]
    public virtual Company? Company { get; set; }
    public Guid? RegionId { get; set; }

    [ForeignKey ("RegionId")]
    public virtual Region? Region { get; set; }
    public string? Note { get; set; }//lý do
   //thông tin về trạng thái khách hàng - học viên

    public CustomerProcessStatus CustomerProcessStatus { get; set; } = CustomerProcessStatus.NotActive; //trạng thái khách hàng mới, liên hệ, đăng ký, học viên, nghỉ học, hủy đăng ký
    public CustomerCheckInStatus CustomerCheckInStatus { get; set; } = CustomerCheckInStatus.NotCheckIn;
    public CustomerStatus CustomerStatus { get; set; } = CustomerStatus.Inactive; //trạng thái khách hàng
    public virtual ICollection<CustomerActivity>? CustomerActivity { get; set; }
}
