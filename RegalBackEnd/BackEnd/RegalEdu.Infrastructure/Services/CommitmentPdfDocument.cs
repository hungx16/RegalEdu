using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Infrastructure.Services
{
    public class CommitmentPdfDocument : IDocument
    {
        private readonly OutputCommitmentPdfViewModel _data;
        private readonly ILocalizationService _loc; // hiện không dùng, giữ để khỏi sửa service

        // Watermark đã được xử lý trong PdfImageHelper (nền trong suốt, scale trước)
        private static readonly Image WatermarkImage =
            PdfImageHelper.LoadTransparentImage("Assets/watermark_regal.png", 0.08f);
        private int fontSizeNormal = 13;
        private int fontSizeHeader = 13;
        public CommitmentPdfDocument(
            OutputCommitmentPdfViewModel data,
            ILocalizationService loc)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _loc = loc ?? throw new ArgumentNullException(nameof(loc));
        }

        public DocumentMetadata GetMetadata() =>
            new DocumentMetadata { Title = "RegalEdu Output Commitment" };

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        [Obsolete]
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // A4 không margin để header/footer bám sát mép
                page.Size(PageSizes.A4);
                page.Margin(0);

                page.DefaultTextStyle(
                    TextStyle.Default
                        .FontFamily("Times New Roman")
                        .FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        #region HEADER / FOOTER

        private void ComposeHeader(IContainer container)
        {
            container
                .Image("Assets/header.png")
                .FitWidth();
        }

        [Obsolete]
        private void ComposeFooter(IContainer container)
        {
            // Hiện tại chỉ dùng ảnh footer, không chèn text
            container
                .PaddingBottom(5)
                .Height(30)
                .Image("Assets/footer.png", ImageScaling.FitWidth);
        }

        #endregion

        #region CONTENT

        // ---------------- CONTENT + WATERMARK ----------------

        private void ComposeContent(IContainer container)
        {
            container
                // chừa khoảng cách với header & footer
                .PaddingLeft(40)
                .PaddingRight(40)
                .PaddingTop(20)
                .PaddingBottom(30)
                .Layers(layers =>
                {
                    // LAYER NỀN: logo watermark chìm giữa trang
                    layers.Layer()
                        .AlignCenter()
                        .AlignMiddle()
                        .Image(WatermarkImage)
                        .FitWidth();

                    // LAYER CHÍNH: nội dung form
                    layers.PrimaryLayer()
                        .Column(col =>
                        {
                            ComposeTitle(col);
                            ComposeStudentSection(col);
                            ComposeCommitmentSection(col);
                        });
                });
        }



        #endregion

        #region TIÊU ĐỀ

        private void ComposeTitle(ColumnDescriptor col)
        {
            // "CAM KẾT ĐẦU RA"
            col.Item().AlignCenter()
                .Text("CAM KẾT ĐẦU RA")
                .FontSize(22)
                .Bold()
                .FontColor("#5A2D82");

            // “Chất lượng là danh dự thương hiệu của Regal Edu”
            col.Item().AlignCenter()
                .Text("“Chất lượng là danh dự thương hiệu của Regal Edu”")
                .FontSize(11)
                .Italic()
                .FontColor("#5A2D82");

            // "A. THÔNG TIN"
            col.Item().PaddingTop(15)
                .Text("A. THÔNG TIN")
                .FontSize(14)
                .Bold()
                .FontColor("#5A2D82");
        }

        #endregion

        #region PHẦN A – THÔNG TIN HỌC VIÊN
        private void AddLabelValueLine(
            ColumnDescriptor column,
            string label,
            string? value)
        {
            // Tách phần Việt / Anh
            var (vi, en) = SplitBiLabel(label);

            column.Item()
                .PaddingBottom(12) // tăng khoảng cách giữa các dòng
                .Text(text =>
                {
                    // Tiếng Việt: in đậm
                    text.Span(vi)
                        .SemiBold()
                        .FontSize(fontSizeNormal)
                        .FontColor("#000000");

                    // Tiếng Anh: chữ thường, cùng dòng
                    if (!string.IsNullOrWhiteSpace(en))
                    {
                        text.Span(en)
                            .FontSize(fontSizeNormal)
                            .FontColor("#000000");
                    }

                    // Nếu có value (dữ liệu) thì nối phía sau
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        text.Span(" ");
                        text.Span(value)
                            .FontSize(fontSizeNormal)
                            .FontColor("#000000");
                    }
                });
        }

        private void ComposeStudentSection(ColumnDescriptor col)
        {
            // Thanh vàng "THÔNG TIN HỌC VIÊN/ Student information:"
            // Tách nhãn song ngữ: Việt / Anh
            var (vi, en) = SplitBiLabel(
                "THÔNG TIN HỌC VIÊN/ Student information:");

            // Thanh vàng header: Việt đậm, Anh thường
            col.Item().PaddingTop(12)
                .Background("#FFE49A")
                .PaddingVertical(4)
                .PaddingHorizontal(6)
                .Text(text =>
                {
                    // Tiếng Việt: in đậm
                    text.Span(vi)
                        .SemiBold()
                        .FontSize(fontSizeNormal)
                        .FontColor("#5A2D82");

                    // Tiếng Anh: thường
                    if (!string.IsNullOrWhiteSpace(en))
                    {
                        text.Span(en)
                            .FontSize(fontSizeNormal)
                            .FontColor("#5A2D82");
                    }
                });
            // Bảng 2 cột: trái và phải
            col.Item().PaddingTop(8).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3); // cột trái
                    c.RelativeColumn(2); // cột phải
                });

                // ===== CỘT TRÁI =====
                t.Cell().Column(left =>
                {
                    AddLabelValueLine(left,
                        "Họ tên học viên/Student's Name:",
                        $"{_data.StudentName}");

                    AddLabelValueLine(left,
                        "Tuổi/Age:",
                        _data.Age);

                    AddLabelValueLine(left,
                        "Số ĐT/Phone:",
                        _data.Phone);

                    AddLabelValueLine(left,
                        "Người liên hệ 1/Parent's name 1:",
                        _data.ParentName1);

                    AddLabelValueLine(left,
                        "Địa chỉ/Address:",
                        _data.ParentAddress1);

                    AddLabelValueLine(left,
                        "Người liên hệ 2/Parent's name 2:",
                        _data.ParentName2);

                    AddLabelValueLine(left,
                        "Địa chỉ/Address:",
                        _data.ParentAddress2);

                });

                // ===== CỘT PHẢI =====
                t.Cell().Column(right =>
                {
                    AddLabelValueLine(right,
                        "Giới tính/Gender:",
                        _data.Gender);

                    AddLabelValueLine(right,
                        "Ngày sinh/Date of Birth:",
                        _data.DateOfBirth);

                    AddLabelValueLine(right,
                        "Email:",
                        _data.Email);

                    AddLabelValueLine(right,
                        "Số ĐT/Phone 1:",
                        _data.ParentPhone1);

                    AddLabelValueLine(right,
                        "Email:",
                        _data.ParentEmail1);

                    AddLabelValueLine(right,
                        "Số ĐT/Phone 2:",
                        _data.ParentPhone2);

                    AddLabelValueLine(right,
                        "Email:",
                        _data.ParentEmail2);
                });
            });

            // Hai dòng tổng học phí và số tháng đăng ký hiển thị full-width để tránh tràn
            col.Item().PaddingTop(3).Column(full =>
            {
                AddLabelValueLine(full,
                    "Tổng học phí đã đóng/Number of registered fees:",
                    _data.TotalFees);

                AddLabelValueLine(full,
                    "Số tháng đăng ký/Number of registered months:",
                    _data.RegisteredMonths);
            });
        }


        #endregion

        #region PHẦN CHƯƠNG TRÌNH CAM KẾT ĐẦU RA

        private void ComposeCommitmentSection(ColumnDescriptor col)
        {
            // Tách nhãn song ngữ: Việt / Anh
            var (vi, en) = SplitBiLabel(
                "CHƯƠNG TRÌNH CAM KẾT ĐẦU RA/ Commitment for the English training programme:");

            // Thanh vàng header: Việt đậm, Anh thường
            col.Item().PaddingTop(5)
                .Background("#FFE49A")
                .PaddingVertical(4)
                .PaddingHorizontal(6)
                .Text(text =>
                {
                    // Tiếng Việt: in đậm
                    text.Span(vi)
                        .SemiBold()
                        .FontSize(fontSizeNormal)
                        .FontColor("#5A2D82");

                    // Tiếng Anh: thường
                    if (!string.IsNullOrWhiteSpace(en))
                    {
                        text.Span(en)
                            .FontSize(fontSizeNormal)
                            .FontColor("#5A2D82");
                    }
                });

            // Đoạn mô tả song ngữ đúng form
            const string paragraph =
                "Học viên được Regal Edu đào tạo đảm bảo đầu ra khi kết thúc lộ trình học, học viên đạt trình độ theo các cấp độ của Cambridge và khung tham chiếu Châu Âu (CEFR)\n" +
                "After finishing the training program, the students will achieve an outcome based on the Cambridge level and the Common European Framework of Reference (CEFR)";

            col.Item().PaddingTop(6)
                .Text(paragraph)
                .LineHeight(1.5f)
                .FontSize(fontSizeNormal)
                .Justify();

            // 3 dòng: Cấp độ bắt đầu / kết thúc / Trình độ cam kết
            col.Item().PaddingTop(10).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(210);  // label
                    c.RelativeColumn();     // ô trống / value
                });

                AddField(t,
                    "Cấp độ bắt đầu/Beginning level:",
                    _data.BeginningLevel);

                AddField(t,
                    "Cấp độ kết thúc/Final level:",
                    _data.FinalLevel);

                AddField(t,
                    "Trình độ cam kết/Output commitment:",
                    _data.OutputCommitmentStatusLabel);
            });
        }


        #endregion

        #region HELPERS
        // Tách nhãn "Tiếng Việt/Tiếng Anh..." thành 2 phần
        private static (string Vi, string En) SplitBiLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                return (string.Empty, string.Empty);

            var parts = label.Split('/', 2);

            if (parts.Length == 2)
            {
                var vi = parts[0];                 // "Họ tên học viên"
                var en = "/" + parts[1];          // "/Student's Name:"
                return (vi, en);
            }

            return (label, string.Empty);
        }



        // Một hàng field có hộp viền tím để điền giá trị
        private void AddField(TableDescriptor t, string label, string? value)
        {
            // Tách phần Việt / Anh
            var (vi, en) = SplitBiLabel(label);

            // LABEL: Việt in đậm, Anh thường, cỡ chữ lớn hơn + dãn cách
            t.Cell().Element(c => c.PaddingVertical(15))   // tăng khoảng cách trên-dưới
                .Text(text =>
                {
                    // Tiếng Việt in đậm
                    text.Span(vi)
                        .SemiBold()
                        .FontSize(fontSizeNormal)
                        .FontColor("#000000");

                    // Tiếng Anh thường (nếu có)
                    if (!string.IsNullOrWhiteSpace(en))
                    {
                        text.Span(en)
                            .FontSize(fontSizeNormal)
                            .FontColor("#000000");
                    }
                });

            // Ô hình chữ nhật: cao hơn, rộng hơn, cỡ chữ to hơn
            t.Cell().Element(c =>
                    c.PaddingVertical(10)       // tăng độ cao ô
                     .PaddingHorizontal(8)
                     .Border(1)
                     .BorderColor("#5A2D82")
                     .MinHeight(24))           // ô chữ nhật cao hơn
                .AlignMiddle()
                .Text(value ?? string.Empty)
                .ParagraphFirstLineIndentation(10)
                .FontSize(fontSizeNormal);
        }


        #endregion
    }
}
