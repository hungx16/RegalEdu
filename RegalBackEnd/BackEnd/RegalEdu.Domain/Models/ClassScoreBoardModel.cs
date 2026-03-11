using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model lưu điểm chi tiết từng nội dung kiểm tra của học viên theo lớp
    public class ClassScoreBoardModel : BaseEntityModel
    {
        public Guid ClassId { get; set; }
        public Class? Class { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public ScoreType ScoreType { get; set; }
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public double? Score { get; set; }
    }
}
