using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Entities
{
    /// <summary>
    /// Entity lưu trữ thông tin về Phiếu Yêu cầu Chuyển Chi nhánh của Học viên.
    /// </summary>
    [Table("TransferCompany")]
    public class TransferCompany : BaseEntity
    {
        // 1. Mã phiếu sinh tự động (TransferCompanyCode)
        [MaxLength(10)]
        public string TransferCompanyCode { get; set; } = string.Empty;

        // --- THÔNG TIN HỌC VIÊN ---
        [MaxLength(10)]
        public string SourceStudentCode { get; set; } = string.Empty;// Mã học viên chuyển
        [MaxLength(200)]
        public string SourceStudentName { get; set; } = string.Empty;// Tên học viên chuyển tính tự động theo mã học viên
        public Guid SourceCompanyId { get; set; }// Chi nhánh hiện tại tự động theo mã học viên
        public Guid? SourceStudentId { get; set; } // EF Core coi Guid là NOT NULL
        [ForeignKey(nameof(SourceStudentId))]
        public virtual Student? SourceStudent { get; set; }

        // 5. Chi nhánh chuyển đến (DestinationCompany)
        public Guid DestinationCompanyId { get; set; }
        [ForeignKey(nameof(DestinationCompanyId))]
        public virtual Company? DestinationCompany { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.Now;// Ngày đến

        [MaxLength(255)]
        public string? Reason { get; set; }// Lý do
        public TransferCompanyStatus TransferCompanyStatus { get; set; } = TransferCompanyStatus.Draft;// Trạng thái
    }
}