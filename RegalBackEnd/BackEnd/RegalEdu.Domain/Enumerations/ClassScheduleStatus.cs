namespace RegalEdu.Domain.Enums
{
    public enum ClassScheduleStatus
    {
        NotStarted = 0,   // Chưa học: Buổi học trong tương lai
        Completed = 1,    // Đã hoàn thành: Ngày học <= ngày hiện tại - 1 và không bị huỷ
        Cancelled = 2     // Huỷ: Buổi học bị huỷ bởi giáo vụ / hệ thống
    }
}
