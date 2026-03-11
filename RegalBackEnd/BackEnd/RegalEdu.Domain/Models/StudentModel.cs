
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class StudentModel : BaseEntityModel
{
    public string StudentCode { get; set; } = string.Empty;//mã học viên tự nhập
    public required string FullName { get; set; } //Họ tên 
    public string? Gender { get; set; } //giới tính nam, nữ, khác
    public DateTime BirthDate { get; set; }//ngày sinh
    public Guid? CategoryId { get; set; } //nhóm tuổi

    [ForeignKey (nameof (CategoryId))]
    public virtual CategoryModel? Category { get; set; } = null!;  //tham chiếu bảng nhóm tuổi

    //Nhân viên tư vấn - liên kêt với bảng nhân viên
    public Guid? EmployeeId { get; set; }

    [ForeignKey ("EmployeeId")]
    public virtual EmployeeModel? Employee { get; set; } //nhân viên tư vấn là nhân viên bán hàng seller và selleader
    public string? Address { get; set; } //địa chỉ 
    public string? Email { get; set; } //
    public string? Phone { get; set; }
    //chi nhánh hiện tại của học sinh - liên kêt với bảng chi nhánh mỗi học sinh thuộc một chi nhánh
    public Guid? CompanyId { get; set; }

    [ForeignKey ("CompanyId")]
    public virtual CompanyModel? Company { get; set; }
    public string? Reason { get; set; }//lý do
    public int? CurrentLevel { get; set; } //Trình độ hiện tại Có các giá trị: Cơ bản/Trước trung câp/Trung cấp/Trung cấp cao/Cao cấp/Chưa rõ
    public int? ExpectedTime { get; set; } //lịch học mong muốn Sáng (8:00-12:00)/Chiều (14:00-18:00)/Tối (18:00-21:00)/Cuối tuần/Linh hoạt
    public string? ExpectedWorkingTime { get; set; } //lịch học mong muốn lấy từ working time lưu theo nguyên tắc “id1#$#id2”.
    public string? LearningGoal { get; set; }//mục tiêu học tập
    public string? EnglishExperience { get; set; } //kinh nghiệm tiếng anh
    public string? LeadSource { get; set; } //có các giá trị: Website /Social /Event /Zalo /Facebook /Tiktok /Support/Khác

    public StudentStatus StudentStatus { get; set; }

    public ICollection<EnrollmentModel>? Enrollments { get; set; } // Danh sách các khóa học mà học sinh đã đăng ký

    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUserModel? ApplicationUser { get; set; } = null!;

    [Required]
    [MaxLength (20)]

    public string? EnglishName { get; set; }
    public string? IdentifyNumber { get; set; }
    public int? Age { get; set; }
    public DateTime? ExpectedStartDate { get; set; } //ngày dự kiến bắt đầu
    public virtual ICollection<RegisterStudyModel>? RegisterStudys { get; set; }
    // Hải: Thêm khóa ngoại trỏ về Category (AgeGroup)
    public virtual ICollection<ContactModel>? Contacts { get; set; }
    public virtual ICollection<CouponModel>? Coupons { get; set; }

    public virtual ICollection<StudentNoteModel>? StudentNote { get; set; } //ghi chú

    public virtual ICollection<StudentActivityModel>? StudentActivity { get; set; } //lịch sử hoạt động
    public virtual ICollection<StudentCourseModel>? StudentCourse { get; set; } //Danh sách các khóa học quan tâm
    public int Priority { get; set; } // độ ưu tiên 
    public double? ExpectedBudget { get; set; }
    public virtual ProfileModel? Profile { get; set; }
    public Guid? RegionId { get; set; }

    [ForeignKey ("RegionId")]
    public virtual RegionModel? Region { get; set; }

    public Guid? LearningRoadMapId { get; set; }
    [ForeignKey ("LearningRoadMapId")]
    public virtual LearningRoadMapModel? LearningRoadMap { get; set; }
    public double? TotalAvailableAmount { get; set; }  // Tổng số tiền dư khả dụng
    //khai báo trường lưu trữ hình ảnh
    public string? AvatarImage { get; set; } // hình đại diện học viên
}
