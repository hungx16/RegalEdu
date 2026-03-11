namespace RegalEdu.Domain.Enumerations
{
    public class AutoCodeInfo
    {
        public required string Prefix { get; set; }
        public required string TableName { get; set; }
        public required string ColumnName { get; set; }
        public int Length { get; set; } = 4;

        //Hải bổ sung thuộc tính AutoCodeInfo dùng cho bảng AllocationEvent
        public string? Format { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
        //vũ bổ sung thuộc tính AutoCodeInfo dùng cho bảng Coupon
        public string? Suffix { get; set; }
        public int? OrderNumber { get; set; }
    }

    public static class AutoCodeConfig
    {
        private static readonly Dictionary<AutoCodeType, AutoCodeInfo> _mapping = new()
        {
            [AutoCodeType.Division] = new AutoCodeInfo { Prefix = "DI", TableName = "Division", ColumnName = "DivisionCode", Length = 4 },
            [AutoCodeType.Department] = new AutoCodeInfo { Prefix = "DE", TableName = "Department", ColumnName = "DepartmentCode", Length = 4 },
            [AutoCodeType.Position] = new AutoCodeInfo { Prefix = "PO", TableName = "Position", ColumnName = "PositionCode", Length = 4 },
            [AutoCodeType.Region] = new AutoCodeInfo { Prefix = "RE", TableName = "Region", ColumnName = "RegionCode", Length = 4 },
            [AutoCodeType.Company] = new AutoCodeInfo { Prefix = "CO", TableName = "Company", ColumnName = "CompanyCode", Length = 4 },
            [AutoCodeType.AgeGroup] = new AutoCodeInfo { Prefix = "AGE", TableName = "Category", ColumnName = "CategoryCode", Length = 4 },
            //Hải sửa
            //[AutoCodeType.Skill] = new AutoCodeInfo { Prefix = "", TableName = "Category", ColumnName = "CategoryCode", Length = 0 },

            //Hải
            //[AutoCodeType.LearningRoadmap] = new AutoCodeInfo { Prefix = "LE", TableName = "LearningRoadmap", ColumnName = "LearningRoadmapCode", Length = 4 },

            [AutoCodeType.Employee] = new AutoCodeInfo { Prefix = "NV", TableName = "AspNetUsers", ColumnName = "UserCode", Length = 4 },

            [AutoCodeType.HolidayType] = new AutoCodeInfo { Prefix = "HT", TableName = "Category", ColumnName = "CategoryCode", Length = 4 },
            [AutoCodeType.ClassType] = new AutoCodeInfo { Prefix = "CT", TableName = "ClassType", ColumnName = "ClassTypeCode", Length = 4 },
            //Hải thêm
            [AutoCodeType.Event_SK] = new AutoCodeInfo { Prefix = "SK", TableName = "Event", ColumnName = "EventCode", Length = 4 },
            [AutoCodeType.Event_BC] = new AutoCodeInfo { Prefix = "BC", TableName = "Event", ColumnName = "EventCode", Length = 4 },
            //[AutoCodeType.Event_TT] = new AutoCodeInfo { Prefix = "TT", TableName = "Event", ColumnName = "EventCode", Length = 4 },
            //[AutoCodeType.Event_LK] = new AutoCodeInfo { Prefix = "LK", TableName = "Event", ColumnName = "EventCode", Length = 4 }
            [AutoCodeType.CourseLesson] = new AutoCodeInfo { Prefix = "Buổi ", TableName = "CourseLesson", ColumnName = "SessionName", Length = 2 },
            //Thiện

            [AutoCodeType.Teacher] = new AutoCodeInfo { Prefix = "GV", TableName = "AspNetUsers", ColumnName = "UserCode", Length = 4 },
            [AutoCodeType.Class] = new AutoCodeInfo { Prefix = "ML", TableName = "Class", ColumnName = "ClassCode", Length = 4 },

            //Hải thêm
            [AutoCodeType.AllocationEvent] = new AutoCodeInfo { Prefix = "PB", TableName = "AllocationEvent", ColumnName = "AllocationCode", Length = 3, Format = "{0}-{1:D4}-{2:D2}-{3}" },
            //Vinh
            [AutoCodeType.Item] = new AutoCodeInfo { Prefix = "AP", TableName = "Item", ColumnName = "ItemCode", Length = 4 },
            //Hải
            [AutoCodeType.TransferCompany] = new AutoCodeInfo { Prefix = "CP", TableName = "TransferCompany", ColumnName = "TransferCompanyCode", Length = 4 },
            // [AutoCodeType.Coupon] = new AutoCodeInfo { Prefix = "AP", TableName = "Coupon", ColumnName = "ItemCode", Length = 4,Suffix= },
            [AutoCodeType.Student] = new AutoCodeInfo { Prefix = "STD", TableName = "AspNetUsers", ColumnName = "UserCode", Length = 4 },
        };

        public static AutoCodeInfo Get(AutoCodeType type) => _mapping[type];
    }

}
