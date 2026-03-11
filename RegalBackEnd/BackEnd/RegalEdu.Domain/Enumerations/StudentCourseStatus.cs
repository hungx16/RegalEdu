
namespace RegalEdu.Domain.Enums
{
    public enum StudentCourseStatus
    {
        NotStarted = 0, // Chưa học
        InProgress = 1, // Đang có lớp (Đã được gán lớp)
        Graduated = 2,//Hoàn thành
        Deferred = 3,//Bảo lưu
        Cancelled = 4, //Hủy
    }

}
