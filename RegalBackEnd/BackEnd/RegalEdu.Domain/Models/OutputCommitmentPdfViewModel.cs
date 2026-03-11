namespace RegalEdu.Domain.Models
{
    public class OutputCommitmentPdfViewModel
    {
        // Thông tin học viên
        public string StudentCode { get; set; } = default!;
        public string StudentName { get; set; } = default!;
        public string? Gender { get; set; }
        public string? Age { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // Phụ huynh – nếu trong Student có thì map, chưa có anh có thể để trống
        public string? ParentName1 { get; set; }
        public string? ParentPhone1 { get; set; }
        public string? ParentEmail1 { get; set; }
        public string? ParentAddress1 { get; set; }

        public string? ParentName2 { get; set; }
        public string? ParentPhone2 { get; set; }
        public string? ParentEmail2 { get; set; }
        public string? ParentAddress2 { get; set; }

        // Học phí & số tháng – tuỳ anh lấy từ đâu
        public string? TotalFees { get; set; }
        public string? RegisteredMonths { get; set; }

        // Cam kết đầu ra
        public string? BeginningLevel { get; set; }
        public string? FinalLevel { get; set; }
        public string? OutputCommitmentInfo { get; set; }
        public string? OutputCommitmentStatusLabel { get; set; }
    }

}
