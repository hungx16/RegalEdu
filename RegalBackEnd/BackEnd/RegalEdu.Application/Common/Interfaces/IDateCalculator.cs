using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Entities;
using System.Linq;

namespace RegalEdu.Application.Common.Services
{
    // Giữ nguyên Interface
    public interface IDateCalculator
    {
        Task<(List<DateTime> Dates, DateTime? EndDate, List<RegalEdu.Domain.Entities.WorkingTime> WorkingTimes)>
            CalculateAsync(
                string classScheduleString,
                DateTime startDate,
                int totalSessions,
                CancellationToken cancellationToken);
    }

    public class DateCalculator : IDateCalculator
    {
        private readonly ILocalizationService _localizer;
        private readonly IRegalEducationDbContext _context;

        public DateCalculator(ILocalizationService localizer, IRegalEducationDbContext context)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<(List<DateTime> Dates, DateTime? EndDate, List<RegalEdu.Domain.Entities.WorkingTime> WorkingTimes)>
            CalculateAsync(
                string classScheduleString,
                DateTime startDate,
                int totalSessions,
                CancellationToken cancellationToken)
        {
            // Kiểm tra tổng số buổi
            if (totalSessions <= 0)
            {
                return (new List<DateTime>(), null, new List<RegalEdu.Domain.Entities.WorkingTime>());
            }

            try
            {
                // 1️ LẤY WORKING TIME (Dùng cách Find/FirstOrDefault như trong AddClass)

                // 1a. Parse danh sách ID WorkingTime
                var workingTimeIds = classScheduleString
                    .Split("#$#", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr => Guid.TryParse(idStr, out var guid) ? guid : Guid.Empty)
                    .Where(guid => guid != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (!workingTimeIds.Any())
                    throw new Exception(_localizer["WorkingTime_Empty"]);

                // 1b. Lấy danh sách WorkingTime từ DB (Truy vấn từng ID để mô phỏng cách AddClass làm)
                var workingTimes = new List<RegalEdu.Domain.Entities.WorkingTime>();
                foreach (var id in workingTimeIds)
                {
                    // Dùng FirstOrDefaultAsync để tương thích tốt hơn với DbContext
                    var wt = await _context.WorkingTimes
                        .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

                    if (wt != null)
                        workingTimes.Add(wt);
                }

                // 1c. Kiểm tra xem có đủ WorkingTime không
                if (workingTimes.Count != workingTimeIds.Count)
                    throw new Exception(_localizer["WorkingTime_Missing"]);

                var daysOfWeek = workingTimes
                    .Select(w => (DayOfWeek)w.DayOfWeek)
                    .Distinct()
                    .ToList();

                if (!daysOfWeek.Any())
                    throw new Exception(_localizer["WorkingTime_NoValidDay"]);

                // 2️ LẤY DANH SÁCH NGÀY NGHỈ LỄ (Giống hệt AddClass)
                var holidaysQuery = await _context.Holidays
                    .Where(h => (h.Frequency == 1 && h.Date.Year == startDate.Year) || h.Frequency == 0)
                    .Select(h => h.Date.Date)
                    .ToListAsync(cancellationToken);

                var holidayDates = new HashSet<DateTime>(holidaysQuery);

                // 3️ TÍNH TOÁN DANH SÁCH NGÀY HỌC CỤ THỂ

                // Tìm ngày học hợp lệ đầu tiên (trong AddClass, nó chỉ tìm ngày học trong tuần, sau đó lọc Holidays trong vòng lặp)
                DateTime currentDate = startDate.Date;

                // Vòng lặp này giúp tìm ngày BẮT ĐẦU học hợp lệ đầu tiên ( >= startDate.Date )
                while (!daysOfWeek.Contains(currentDate.DayOfWeek) || holidayDates.Contains(currentDate))
                {
                    currentDate = currentDate.AddDays(1);
                }

                // 4️ Dò từng ngày, đếm số buổi còn lại (Giống hệt logic AddClass)
                var resultDates = new List<DateTime>();
                int sessionsCounted = 0;

                while (sessionsCounted < totalSessions)
                {
                    // Chỉ thêm vào danh sách nếu là ngày học trong tuần VÀ KHÔNG phải ngày nghỉ lễ
                    if (daysOfWeek.Contains(currentDate.DayOfWeek) && !holidayDates.Contains(currentDate))
                    {
                        resultDates.Add(currentDate);
                        sessionsCounted++;

                        // Break sớm nếu đã đủ (cách này an toàn hơn so với việc để vòng lặp while tự kiểm tra)
                        if (sessionsCounted == totalSessions)
                            break;
                    }

                    currentDate = currentDate.AddDays(1);
                }

                DateTime? endDate = resultDates.LastOrDefault();

                return (resultDates, endDate, workingTimes);
            }
            catch (Exception ex)
            {
                // Ghi log ngoại lệ (Quan trọng để debug nguyên nhân lỗi)
                // Logger?.LogError(ex, "Lỗi khi tính toán lịch học trong DateCalculator");

                // Trả về kết quả trống, kích hoạt lỗi trong Handler gọi (như đã xảy ra)
                return (new List<DateTime>(), null, new List<RegalEdu.Domain.Entities.WorkingTime>());
            }
        }
    }
}