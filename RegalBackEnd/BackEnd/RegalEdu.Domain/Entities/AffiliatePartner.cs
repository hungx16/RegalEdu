using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("AffiliatePartner")]
    [Index (nameof (PartnerCode), IsUnique = true)] // Unique theo yêu cầu "Mã định danh duy nhất"
    public class AffiliatePartner : BaseEntity
    {
        // FK: PartnerType
        [Required]
        public Guid PartnerTypeId { get; set; }

        [ForeignKey (nameof (PartnerTypeId))]
        public virtual PartnerType? PartnerType { get; set; }

        /// <summary>
        /// Mã định danh duy nhất cho đối tác (VD: TH001)
        /// </summary>
        [Required]
        [MaxLength (50)]
        public string PartnerCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên đầy đủ của đối tác
        /// </summary>
        [Required]
        [MaxLength (200)]
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Vị trí địa lý/phạm vi (VD: bán kính dưới 5km)
        /// </summary>
        [MaxLength (200)]
        public string? AgencyLocation { get; set; }

        /// <summary>
        /// Khu vực (Tỉnh/Thành phố)
        /// </summary>
        [Required]
        [MaxLength (100)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ chi tiết
        /// </summary>
        [MaxLength (300)]
        public string? Address { get; set; }

        /// <summary>
        /// Họ tên người đại diện
        /// </summary>
        [Required]
        [MaxLength (100)]
        public string ContactPerson { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        [Required]
        [MaxLength (15)]
        [Column (TypeName = "varchar(15)")] // theo mô tả: Phone là varchar(15)
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Chức vụ của người đại diện
        /// </summary>
        [Required]
        [MaxLength (100)]
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Đăng lên website hay không
        /// </summary>
        public bool IsPublish { get; set; } = false;


        public virtual Image? Image { get; set; } = null!;

        public string EnPartnerName { get; set; } = string.Empty;
        public bool IsMultilingual { get; set; }
        public int SortOrder { get; set; }

        public SchoolLevelType SchoolLevel { get; set; }
        public bool IsFinancialCompany { get; set; }
        public string? WebsiteKeys { get; set; }
        [Column (TypeName = "decimal(18,1)")]
        public float InterestRate { get; set; }
        public int LoanTerm { get; set; }
        public string? LoanBenefits { get; set; }
        public string? EnLoanBenefits { get; set; }
        public string? EnWebsiteKeys { get; set; }


    }
}
