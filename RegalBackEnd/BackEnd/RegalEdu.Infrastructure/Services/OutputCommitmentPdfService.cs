using QuestPDF.Fluent;
using RegalEdu.Domain.Models;

namespace RegalEdu.Infrastructure.Services
{
    using Microsoft.EntityFrameworkCore;
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using RegalEdu.Application.Common.Interfaces;
    using RegalEdu.Domain.Enumerations;

    public class OutputCommitmentPdfService : IOutputCommitmentPdfService
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localization;

        public OutputCommitmentPdfService(
            IRegalEducationDbContext repo,
            ILocalizationService localization)
        {
            _context = repo;
            _localization = localization;
        }

        public async Task<byte[]> GeneratePdfAsync(OutputCommitmentModel model, CancellationToken ct = default)
        {
            var entity = await _context.Students.Include(t => t.Contacts).Where(t => t.Id.ToString() == model.StudentId.ToString()).FirstOrDefaultAsync();
            if (entity == null)
                throw new KeyNotFoundException("Output commitment not found");

            var s = entity;

            var vm = new OutputCommitmentPdfViewModel
            {
                StudentCode = entity.StudentCode,
                StudentName = s?.FullName ?? "",
                Gender = s?.Gender,
                Age = CalculateAgeText(s?.BirthDate),

                DateOfBirth = s?.BirthDate.ToString("dd/MM/yyyy"),
                Phone = s?.Phone,
                Email = s?.Email,
            };

            if (s.Contacts != null && s.Contacts.Any())
            {
                var parent = s.Contacts.First();
                vm.ParentName1 = parent.FullName;
                vm.ParentPhone1 = parent.Phone;
                vm.ParentAddress1 = parent.Address;
            }

            vm.TotalFees = model.TotalRegisteredFee.ToString("N0");
            vm.RegisteredMonths = model.TotalRegisteredMonths.ToString();

            vm.BeginningLevel = model.BeginningLevel;
            vm.FinalLevel = model.FinalLevel;
            vm.OutputCommitmentInfo = model.OutputCommitmentInfo;
            vm.OutputCommitmentStatusLabel = MapStatus(model.OutputCommitmentStatus);

            var doc = new CommitmentPdfDocument(vm, _localization);
            var mainPdfBytes = doc.GeneratePdf();

            // 2. Tạo document kết quả
            using var outputDoc = new PdfDocument();

            // Helper: copy toàn bộ page từ 1 PdfDocument sang output
            void ImportAllPages(PdfDocument src)
            {
                for (int i = 0; i < src.PageCount; i++)
                {
                    outputDoc.AddPage(src.Pages[i]);
                }
            }

            // 3. Import PDF trang 1 (QuestPDF) từ byte[]
            using (var mainStream = new MemoryStream(mainPdfBytes))
            {
                var mainDoc = PdfReader.Open(mainStream, PdfDocumentOpenMode.Import);
                ImportAllPages(mainDoc);
            }

            // 4. Import trang 2–3 từ 1 file template
            var templatePath = Path.Combine("Assets", "Templates", "RegalEdu_OutputCommitment.pdf");
            if (File.Exists(templatePath))
            {
                var templateDoc = PdfReader.Open(templatePath, PdfDocumentOpenMode.Import);

                // 📝 Lưu ý: PageIndex là 0-based
                // Page 0 = trang 1 trong file; Page 1 = trang 2

                // Nếu muốn lấy TẤT CẢ các trang:
                // ImportAllPages(templateDoc);

                // Nếu chỉ muốn lấy 2 trang đầu:
                var max = Math.Min(2, templateDoc.PageCount);
                for (int i = 0; i < max; i++)
                {
                    outputDoc.AddPage(templateDoc.Pages[i]);
                }
            }

            // 5. Xuất document ghép xong ra byte[]
            using var outStream = new MemoryStream();
            outputDoc.Save(outStream, false);

            return outStream.ToArray();
        }
        private static string? CalculateAgeText(DateTime? dateOfBirth)
        {
            if (dateOfBirth == null)
                return null;

            var today = DateTime.Today;
            var dob = dateOfBirth.Value.Date;

            var age = today.Year - dob.Year;

            // Nếu hôm nay chưa qua ngày sinh nhật năm nay → trừ đi 1
            if (dob > today.AddYears(-age))
                age--;

            return age < 0 ? null : age.ToString();
        }

        private string MapStatus(OutputCommitmentStatus status)
        {
            // dùng resource Messages
            return status switch
            {


                OutputCommitmentStatus.Finished =>
                    _localization["OutputCommitment_Status_Finished"],

                OutputCommitmentStatus.NotFinished =>
                    _localization["OutputCommitment_Status_NotFinished"],

                _ => status.ToString()
            };
        }
    }


}
