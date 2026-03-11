export interface HeaderParam {
	label: string;
	type: string;
	placeholder: string;
	data: null;
}

export interface BtnFunction {
	label: string;
	type: string;
	event: string;
	translatedLabel?: string;
}

export interface ListHeaderParams {
	listParams: Array<HeaderParam>;
	listBtn: Array<BtnFunction>;
}
export enum StatusType {
	Active = 0,
	Inactive = 1,
	Deleted = 2,
}
export enum ActionType {
	Create = 'create',
	Edit = 'edit',
	Delete = 'delete'
}
export enum FrequencyType {
	Once = 0,
	Weekly = 1,
	Monthly = 2,
	Yearly = 3
}
export enum CategoryType {
	AgeGroup = 1,   // Nhóm tuổi
	Source = 2,     // Nguồn
	Skill = 3,      // Kỹ năng
	HolidayType = 4 // Loại nghỉ lễ
}


export enum QuotaStatus {
	Draft = 0,
	Allocated = 1,
	InProgress = 2,
	Completed = 3,
}
export enum AdjustmentScope {
	Region = 1,
	Company = 2,
}
export enum QuotaRole {
	ASM = 1,
	BM = 2,
	SalesLead = 3,
	Sale = 4,
	Support = 5,
	ProbationEmployee = 6,
	LeavingEmployee = 7,
}

export enum WorkStage {
	Normal = 0,
	EndingThisMonth = 4,
	ProbationEnd = 2,
	ProbationStart = 1,
	ProbationStartEnd = 3,
}

export enum UnitType {
	Hour = 0,
	Session = 1,
	Month = 2,
	Course = 3,
}

export enum CommitmentOutputType {
	None = 0,
	Included = 1,
	SelfCommitment = 2,
}
export const CommitmentOutputTypeLabels: Record<CommitmentOutputType, string> = {
	[CommitmentOutputType.None]: "course.commitmentOutputTypes.none",
	[CommitmentOutputType.Included]: "course.commitmentOutputTypes.included",
	[CommitmentOutputType.SelfCommitment]: "course.commitmentOutputTypes.selfCommitment",
};

export enum PriceTargetType {
	Course = 0,
	Program = 1,
}
export enum EventCategory {
	Event = 0,   // Sự kiện
	Report = 1,  // Báo cáo
	//	News = 2,    // Tin tức
	//	Link = 3     // Liên kết
}

// Map enum -> i18n key
export const EventCategoryLabels: Record<EventCategory, string> = {
	[EventCategory.Event]: "eventCategory.event",
	[EventCategory.Report]: "eventCategory.report",
	//	[EventCategory.News]: "eventCategory.news",
	//	[EventCategory.Link]: "eventCategory.link",
};

export enum PromotionType {
	Discount = 1,
	Gift = 2,
	Coupon = 3,
	FixedPrice = 4
}
export enum DiscountMeThod {
	OrderTotal = 0, // theo tổng đơn hàng
	Quantity = 1, // theo số lượng sản phẩm
}

export enum FormatType {
	Pdf = 0,
	Word = 1,
	Audio = 2,
	Video = 3,
	Image = 4
}
export const FormatTypeLabels: Record<FormatType, string> = {
	[FormatType.Pdf]: "formatType.pdf",
	[FormatType.Word]: "formatType.word",
	[FormatType.Audio]: "formatType.audio",
	[FormatType.Video]: "formatType.video",
	[FormatType.Image]: "formatType.image",
};
export enum TopicType {
	Grammar = 0,
	Vocabulary = 1,
	Listening = 2,
	Reading = 3,
	Speaking = 4,
	Writing = 5
}
export const TopicTypeLabels: Record<TopicType, string> = {
	[TopicType.Grammar]: "topicType.grammar",
	[TopicType.Vocabulary]: "topicType.vocabulary",
	[TopicType.Listening]: "topicType.listening",
	[TopicType.Reading]: "topicType.reading",
	[TopicType.Speaking]: "topicType.speaking",
	[TopicType.Writing]: "topicType.writing",
};

export enum ApplicationUserType {
	Teacher = 0,
	Employee = 1,
}
export const ApplicationUserTypeLabels: Record<ApplicationUserType, string> = {
	[ApplicationUserType.Teacher]: "applicationUserType.teacher",
	[ApplicationUserType.Employee]: "applicationUserType.employee",
};

export enum WorkType {
	FullTime = 0,
	PartTime = 1,
	Contract = 2
}
export const WorkTypeLabels: Record<WorkType, string> = {
	[WorkType.FullTime]: "workType.fullTime",
	[WorkType.PartTime]: "workType.partTime",
	[WorkType.Contract]: "workType.contract",
};

export enum LevelType {
	Beginner = 1,       // Mới bắt đầu
	Basic = 2,          // Cơ bản
	Intermediate = 3,   // Trung cấp
	UpperIntermediate = 4, // Trung cấp cao
	Advanced = 5,       // Nâng cao
	Proficient = 6      // Thành thạo
}
export const LevelTypeLabels: Record<LevelType, string> = {
	[LevelType.Beginner]: "levelType.beginner",
	[LevelType.Intermediate]: "levelType.intermediate",
	[LevelType.Advanced]: "levelType.advanced",
	[LevelType.Basic]: "levelType.basic",
	[LevelType.UpperIntermediate]: "levelType.upperIntermediate",
	[LevelType.Proficient]: "levelType.proficient"
};
export enum ClassStatus {
	Plan = 0,        // Kế hoạch
	InProgress = 1,  // Đang học
	Completed = 2    // Hoàn thành
}
export const ClassStatusLabels: Record<ClassStatus, string> = {
	[ClassStatus.Plan]: "classStatus.plan",
	[ClassStatus.InProgress]: "classStatus.inProgress",
	[ClassStatus.Completed]: "classStatus.completed",
};

export enum ClassMethod {
	Onsite = 0,
	Online = 1
};
export const ClassMethodLabels: Record<ClassMethod, string> = {
	[ClassMethod.Onsite]: "classMethod.onsite",
	[ClassMethod.Online]: "classMethod.online",
};
export enum AllocationEventStatus {
	Draft = 0,        // Nháp
	Allocated = 1,    // Đã phân bổ
	Completed = 2,    // Hoàn thành
	Cancelled = 3     // Huỷ
};
export const AllocationEventStatusLabels: Record<AllocationEventStatus, string> = {
	[AllocationEventStatus.Draft]: "allocationEventStatus.statusDraft",
	[AllocationEventStatus.Allocated]: "allocationEventStatus.statusAllocated",
	[AllocationEventStatus.Completed]: "allocationEventStatus.statusCompleted",
	[AllocationEventStatus.Cancelled]: "allocationEventStatus.statusCancelled",
};

export enum CompanyEventProposalStatus {
	Draft = 0,
	PendingApproval = 1,
	Approved = 2,
	Rejected = 3,
}

export const CompanyEventProposalStatusLabels: Record<CompanyEventProposalStatus, string> = {
	[CompanyEventProposalStatus.Draft]: "companyEventProposalStatus.draft",
	[CompanyEventProposalStatus.PendingApproval]: "companyEventProposalStatus.pending",
	[CompanyEventProposalStatus.Approved]: "companyEventProposalStatus.approved",
	[CompanyEventProposalStatus.Rejected]: "companyEventProposalStatus.rejected",
};

export enum StudentStatus {
	Prospect = 0,       // Ti?m n?ng
	Enrolled = 1,       // ?? nh?p h?c / ?ang h?c
	Paused = 2,         // T?m d?ng
	Dropped = 3,        // B? h?c
	Graduated = 4,      // T?t nghi?p
	NewRegister = 5,    // ??ng k? m?i
}

export enum ClassScheduleStatus {
	NotStarted = 0,   // Chưa học
	Completed = 1,    // Đã hoàn thành
	Cancelled = 2     // Huỷ
}

export enum SessionAttendanceStatus {
	NotChecked = 0,   // Chưa điểm danh
	Checked = 1,      // Đã điểm danh
	Confirmed = 2     // Đã xác nhận
}

export enum SessionAttendanceLockStatus {
	Unlocked = 0,   // Không khóa điểm danh
	Locked = 1      // Đã khóa điểm danh
}

export enum StudentParticipationStatus {
	Absent = 0,   // Vắng mặt
	Present = 1   // Có mặt
}

export enum StudentHomeworkStatus {
	NotDone = 0,  // Chưa làm bài tập
	Done = 1      // Đã làm bài tập
}
export enum LeadSource {
	Website = "Website",       // Tiềm năng
	Social = "Social",       // Đã nhập học / Đang học
	Zalo = "Zalo",         // Tạm dừng
	Facebook = "Facebook",       // Bỏ học
	Event = "Event",     // Tốt nghiệp
	Tiktok = "Tiktok",
	Support = "Support",
	Other = "Other" // Đăng ký mới
}
export enum Relationship {
	Father = 0,
	Mother = 1,
	Sister = 2,
	Brother = 3,
	Other = 4
}
export enum activityType {
	Call = "0",       // Cuộc gọi
	Sms = "1",       // Tin nhắn
	Zalo = "2",         // Tạm dừng
	Email = "3",       // Bỏ học
	Event = "4",     // Tốt nghiệp
	Other = "5" // Đăng ký mới
}

export enum InterestLevel {
	Low = "0",       // Thấp
	Medium = "1",       // Trung bình
	High = "2",         // Cao
}

export enum PaymentStatus {
	Paid = 0,       // Đã thanh toán
	PartiallyPaid = 1,       // Thanh toán một phần
	Unpaid = 2         // Chưa thanh toán
}
export enum PaymentType {
	Direct = 0, // Trả thẳng
	Installment = 1, // Trả góp
}
export enum ReceiptType {
	Order = 0,
	Additional = 1,
	Deposit = 2,
}
export enum PaymentMeThod {
	Cash = 0,
	Transfer = 1,
	VnPay = 2,
}
export enum PaymentMethodType {
	OneTime = 0,
	Multiple = 1,

}
export enum ExpectedTime {
	Morning = 0,
	Afternoon = 1,
	Evening = 2,
	Weekend = 3,
	Other = 4,
}
export enum CurrentLevel {
	PreIntermediate = 0,
	Intermediate = 1,
	UpperIntermediate = 2,
	Advanced = 3,
	Unknown = 4,

}
export enum EventSize {
	Mini = 0,
	Big = 1,
}


// Phát hành coupon
export enum IssueType {
	Quantity = 0, // theo số lượng
	SelectedStudent = 1, // theo học sinh được chọn
}
// Phát hành coupon
export enum IssueStatus {
	NotActive = 0, // chưa kích hoạt
	Active = 1, // đã kích hoạt
	Expired = 2, // đã hết hạn
}
export enum CouponStatus {
	NotUsed = 0, // chưa sử dụng
	Used = 1, // đã sử dụng
	Expired = 2, // đã hết hạn
}
export enum DueType {
	duration = 0, // theo số ngày
	dateRange = 1, // theo khoảng thời gian start - end
}
export enum CouponTypeStatus {
	NotActive = 0, // Chưa kích hoạt
	Active = 1, // Đã kích hoạt
	Suspended = 2, // Đã tạm ngưng
}

export enum OutputCommitmentStatus {
	NotFinished = 0,
	Finished = 1,
}

export enum SchoolLevelType {
	Preschool = 0,
	PrimarySchool = 1,
	SecondarySchool = 2,
	HighSchool = 3,
}
export const SchoolLevelTypeLabels: Record<SchoolLevelType, string> = {
	[SchoolLevelType.Preschool]: "schoolLevelType.preschool",
	[SchoolLevelType.PrimarySchool]: "schoolLevelType.primarySchool",
	[SchoolLevelType.SecondarySchool]: "schoolLevelType.secondarySchool",
	[SchoolLevelType.HighSchool]: "schoolLevelType.highSchool",
};
