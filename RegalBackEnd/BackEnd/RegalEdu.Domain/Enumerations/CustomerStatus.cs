
namespace RegalEdu.Domain.Enums
{
    public enum CustomerStatus
    {
        NotLead = 0, // Không tiềm năng
        Lead = 1, // tiềm năng
        Prospect = 2, //khách hàng tìm hiểu
        Contacted = 3,// quan tâm đã được tư vấn liên hệ
        Converted = 4,//đã chuyển đổi đã đăng ký học
        Retained = 5, //giữ chân đăng ký lại trung thành
        Inactive = 6 //không hoạt động
    }

}
