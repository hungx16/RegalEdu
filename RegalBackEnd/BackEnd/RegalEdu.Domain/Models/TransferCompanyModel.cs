using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums; // Đảm bảo bạn có namespace này
using RegalEdu.Domain.Models; // Giả sử các Model liên quan cũng nằm trong namespace này
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    /// <summary>
    /// Model lưu trữ thông tin về Phiếu Yêu cầu Chuyển Chi nhánh của Học viên.
    /// </summary>    
    public class TransferCompanyModel : BaseEntityModel
    {
        // 1. Mã phiếu sinh tự động (TransferCompanyCode)
        [MaxLength(10)]
        public string TransferCompanyCode { get; set; } = string.Empty;

        // --- THÔNG TIN HỌC VIÊN ---
        [MaxLength(10)]
        public string SourceStudentCode { get; set; } = string.Empty;
        [MaxLength(200)]
        public string SourceStudentName { get; set; } = string.Empty;// Ko cho client nhập
        public Guid SourceCompanyId { get; set; }//Không cho client nhập
        public Guid? SourceStudentId { get; set; } // EF Core coi Guid là NOT NULL
        [ForeignKey(nameof(SourceStudentId))]
        public virtual Student? SourceStudent { get; set; }

        // 5. Chi nhánh chuyển đến (DestinationCompany)
        public Guid DestinationCompanyId { get; set; }
        [ForeignKey(nameof(DestinationCompanyId))]
        public virtual Company? DestinationCompany { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? Reason { get; set; }
        public TransferCompanyStatus TransferCompanyStatus { get; set; } = TransferCompanyStatus.Draft;
    }
}