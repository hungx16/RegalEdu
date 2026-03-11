using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Common
{
    public static class AutoCodeHelper
    {
        public static async Task<TResult> CreateWithAutoCodeRetryAsync<TResult>(
            AutoCodeInfo info,
            Func<string, Task<TResult>> insertFunc,
            DbContext dbContext, // hoặc IRegalEducationDbContext context, tuỳ kiến trúc
            int maxRetry = 5)
        {
            for (int i = 0; i < maxRetry; i++)
            {
                var code = await GenerateCodeAsync (info, dbContext);
                try
                {
                    return await insertFunc (code);
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation (ex))
                {
                    continue;
                }
            }
            throw new Exception ("EX_TOO_MANY_RETRY");
        }


        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains ("UNIQUE") == true;
        }

        public static async Task<string> GenerateCodeAsync(
            AutoCodeInfo info,
            DbContext dbContext
        //Guid? companyId = null,  // Thêm Id công ty để lấy mã chi nhánh
        //Guid? courseId = null    // Thêm Id khoá học để lấy mã khoá học
        )
        {
            // 🧩 Hải bổ sung — câu SQL tùy theo loại bảng
            string sql;

            if (info.TableName.Equals ("AllocationEvent", StringComparison.OrdinalIgnoreCase))
            {
                // Với AllocationEvent → sắp xếp theo số cuối
                sql = $@"
                SELECT TOP 1 [{info.ColumnName}] AS [Value]
                FROM [{info.TableName}]
                WHERE [{info.ColumnName}] LIKE @Prefix + '%'
                ORDER BY TRY_CAST(RIGHT([{info.ColumnName}], {info.Length}) AS INT) DESC";
            }
            //else if (info.TableName.Equals("Class", StringComparison.OrdinalIgnoreCase))
            //{
            //    if (!companyId.HasValue || !courseId.HasValue)
            //        throw new ArgumentException("CompanyId and CourseId must be provided for Class code generation.");

            //    var companyCode = await dbContext.Set<RegalEdu.Domain.Entities.Company>()
            //        .Where(c => c.Id == companyId && !c.IsDeleted)
            //        .Select(c => c.CompanyCode)
            //        .FirstOrDefaultAsync();

            //    var courseCode = await dbContext.Set<RegalEdu.Domain.Entities.Course>()
            //        .Where(c => c.Id == courseId && !c.IsDeleted)
            //        .Select(c => c.CourseCode)
            //        .FirstOrDefaultAsync();

            //    if (string.IsNullOrWhiteSpace(companyCode) || string.IsNullOrWhiteSpace(courseCode))
            //        throw new Exception("Cannot find CompanyCode or CourseCode.");

            //    info.Prefix = $"{companyCode}_{courseCode}_";

            //    // Câu SQL vẫn dùng để lấy STT cuối cùng
            //    sql = $@"
            //    SELECT TOP 1 [{info.ColumnName}] AS [Value]
            //    FROM [{info.TableName}]
            //    ORDER BY TRY_CAST(RIGHT([{info.ColumnName}], {info.Length}) AS INT) DESC";
            //}

            else if(info.TableName.Equals("Coupon", StringComparison.OrdinalIgnoreCase))
            {
                sql = $@"
                SELECT TOP 1 [{info.ColumnName}] AS [Value]
                FROM [{info.TableName}]
                WHERE [{info.ColumnName}] LIKE @Prefix + '%'
                ORDER BY TRY_CAST(SUBSTRING([{info.ColumnName}],{info.Prefix.Length+1}, {info.Length}) AS INT) DESC";
            }
            else
            {
                // Với bảng khác → vẫn sắp xếp theo chuỗi như cũ
                sql = $@"
                SELECT TOP 1 [{info.ColumnName}] AS [Value]
                FROM [{info.TableName}]
                WHERE [{info.ColumnName}] LIKE @Prefix + '%'
                ORDER BY [{info.ColumnName}] DESC";
            }

            var param = new Microsoft.Data.SqlClient.SqlParameter ("@Prefix", info.Prefix);
            var lastCode = (await dbContext.Database
                .SqlQueryRaw<string> (sql, param)
                .FirstOrDefaultAsync ( )) ?? "";

            int nextNumber = 1;
            //int oridinNumber = 0;
            if (!string.IsNullOrEmpty (lastCode))
                // Hải bổ sung — xử lý riêng cho AllocationEvent
                if (info.TableName.Equals ("AllocationEvent", StringComparison.OrdinalIgnoreCase))
                {
                    var lastPart = lastCode.Split ('-').LastOrDefault ( );
                    if (int.TryParse (lastPart, out int parsed))
                        nextNumber = parsed + 1;
                }
                else if (info.TableName.Equals("Coupon", StringComparison.OrdinalIgnoreCase))
                {
                   
                    nextNumber = int.Parse(lastCode.Substring(info.Prefix.Length,info.Length)) + 1 + (int)info.OrderNumber;

                }
                else
                {
                    nextNumber = int.Parse (lastCode.Substring (info.Prefix.Length)) + 1;
                }

            // Trường hợp đặc biệt: AllocationEvent có định dạng riêng
            if (info.TableName.Equals ("AllocationEvent", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace (info.Format))
            {
                int year = info.Year ?? DateTime.Now.Year;
                //int month = info.Month ?? DateTime.Now.Month;
                // ❌ Không mặc định tháng bằng tháng hiện tại
                if (info.Month == null)
                    throw new ArgumentException ("Month must be provided for AllocationEvent.");

                int month = info.Month.Value;

                return string.Format (
                    info.Format,
                    info.Prefix,
                    year,
                    month,
                    nextNumber.ToString ( ).PadLeft (info.Length, '0')
                );
            }
            // vũ bổ sung mã coupon có hậu tố
            if (info.TableName.Equals("Coupon", StringComparison.OrdinalIgnoreCase))
            {
                return $"{info.Prefix}{nextNumber.ToString().PadLeft(info.Length, '0')}{info.Suffix}";
            }

            return $"{info.Prefix}{nextNumber.ToString ( ).PadLeft (info.Length, '0')}";
        }

    }
}
