namespace RegalEdu.Shared
{
    public class Functions
    {
        public static string GetFullExceptionMessage(Exception ex)
        {
            if (ex == null) return string.Empty;

            var messages = new List<string> { ex.Message };
            var inner = ex.InnerException;

            while (inner != null)
            {
                messages.Add (inner.Message);
                inner = inner.InnerException;
            }

            return string.Join (" --> ", messages);
        }
        public static string CapitalizeFirstLetter(string sentence)
        {
            if (string.IsNullOrWhiteSpace (sentence))
                return sentence;

            sentence = sentence.Trim ( );
            return char.ToUpper (sentence[0]) + sentence.Substring (1);
        }

        public static string GenerateSecurePassword( )
        {
            return Guid.NewGuid ( ).ToString ("N").Substring (0, 12) + "!Aa1";
        }
        public static bool IsWithinMonth(DateTime day, int year, int month)
    => day.Year == year && day.Month == month;

        public static DateTime LastDayOfMonth(int year, int month)
            => new DateTime (year, month, DateTime.DaysInMonth (year, month));

        /// <summary>Đếm số ngày làm việc trong tháng (loại Chủ nhật).</summary>
        public static int CountWorkingDaysInMonth(int year, int month)
        {
            var start = new DateTime (year, month, 1);
            var end = LastDayOfMonth (year, month);
            return CountWorkingDaysBetween (start, end);
        }

        /// <summary>Đếm số ngày làm việc trong khoảng (bao gồm 2 đầu mút), loại Chủ nhật.</summary>
        public static int CountWorkingDaysBetween(DateTime startInclusive, DateTime endInclusive)
        {
            if (endInclusive < startInclusive) return 0;
            var cnt = 0;
            for (var d = startInclusive.Date; d <= endInclusive.Date; d = d.AddDays (1))
            {
                if (d.DayOfWeek != DayOfWeek.Sunday) cnt++;
            }
            return cnt;
        }

        /// <summary>Calculate D_work using 6-1 schedule for an inclusive range.</summary>
        public static int CalculateDWork(DateTime startInclusive, DateTime endInclusive)
        {
            var start = startInclusive.Date;
            var end = endInclusive.Date;
            if (end < start) return 0;

            var n = (end - start).Days + 1;
            var fullWeeks = n / 7;
            var remaining = n % 7;
            return (fullWeeks * 6) + Math.Min (6, remaining);
        }

        /// <summary>Calculate D_prob by summing D_work per month from start date to as-of date.</summary>
        public static int CalculateDProb(DateTime startDate, DateTime asOfDate)
        {
            var start = startDate.Date;
            var asOf = asOfDate.Date;
            if (asOf < start) return 0;

            var startMonthStart = new DateTime (start.Year, start.Month, 1);
            var asOfMonthStart = new DateTime (asOf.Year, asOf.Month, 1);

            var firstMonthEnd = LastDayOfMonth (start.Year, start.Month);
            var firstEnd = asOf < firstMonthEnd ? asOf : firstMonthEnd;
            var total = CalculateDWork (start, firstEnd);

            var monthCursor = startMonthStart.AddMonths (1);
            while (monthCursor < asOfMonthStart)
            {
                var monthEnd = LastDayOfMonth (monthCursor.Year, monthCursor.Month);
                total += CalculateDWork (monthCursor, monthEnd);
                monthCursor = monthCursor.AddMonths (1);
            }

            if (startMonthStart != asOfMonthStart)
                total += CalculateDWork (asOfMonthStart, asOf);

            return total;
        }

        /// <summary>Get D_std based on month length.</summary>
        public static int GetStandardWorkingDaysInMonth(int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth (year, month);
            return daysInMonth switch
            {
                30 => 26,
                31 => 27,
                28 => 24,
                29 => 25,
                _ => 0
            };
        }

        /// <summary>Calculate D_60 and D_100 for probation threshold (26 days).</summary>
        public static (int D60, int D100) CalculateProbationDays(int dWork, int dProb)
        {
            if (dWork <= 0) return (0, 0);

            var prob = Math.Max (0, dProb);
            var remaining = 26 - prob;
            var d60 = Math.Max (0, Math.Min (dWork, remaining));
            var d100 = dWork - d60;
            return (d60, d100);
        }

        /// <summary>Calculate probation quota using D_work, D_prob, and D_std.</summary>
        public static decimal CalculateProbationQuota(decimal totalQuota, int dStd, int dWork, int dProb)
        {
            if (dStd <= 0 || dWork < 7 || totalQuota <= 0) return 0m;

            var (d60, d100) = CalculateProbationDays (dWork, dProb);
            var perDay = totalQuota / dStd;
            return perDay * ((0.6m * d60) + (1.0m * d100));
        }

        /// <summary>Calculate sales quota using D_work for leaving staff.</summary>
        public static decimal CalculateLeavingSalesQuota(decimal totalQuota, int dStd, DateTime startDate, DateTime endDate)
        {
            var dWork = CalculateDWork (startDate, endDate);
            return CalculateSalesQuota (totalQuota, dStd, dWork);
        }

        /// <summary>Calculate sales quota using a D_work value.</summary>
        public static decimal CalculateSalesQuota(decimal totalQuota, int dStd, int dWork)
        {
            if (dStd <= 0 || dWork <= 0 || totalQuota <= 0) return 0m;
            var perDay = totalQuota / dStd;
            return perDay * dWork;
        }

    }
}
