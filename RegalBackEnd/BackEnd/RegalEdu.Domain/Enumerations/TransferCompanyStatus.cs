namespace RegalEdu.Domain.Enums
{
    public enum TransferCompanyStatus
    {
        Draft = 1,                 // Nháp: Chi nhánh đã tạo nhưng chưa gửi đi.
        PendingApproval = 2,       // Chờ phê duyệt: Chi nhánh đã gửi, chờ Học vụ duyệt.
        Rejected = 3,              // Từ chối: Phòng Học vụ từ chối yêu cầu.
        Approved = 4,              // Phê duyệt: Phòng Học vụ đã phê duyệt, chờ Phụ huynh xác nhận.
        ParentConfirmed = 5,       // Phụ huynh xác nhận: Phụ huynh đã xác nhận yêu cầu.
        ParentRejected = 6,        // Phụ huynh từ chối: Phụ huynh từ chối yêu cầu.
        Completed = 7,             // Hoàn thành: Yêu cầu đã được xử lý xong sau khi Phụ huynh xác nhận (hoặc trạng thái cuối).
    }
}