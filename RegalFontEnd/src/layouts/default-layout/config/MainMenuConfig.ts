import type { MenuItem } from "@/layouts/default-layout/config/types";

const MainMenuConfig: Array<MenuItem> = [
  // ===== CẤP 1: Dashboard (không cần section con) =====
  {
    pages: [
      {
        heading: "models.Dashboard",
        route: "dashboard",
        bootstrapIcon: "bi-house-up",
      },
      {
        heading: "models.BeeAI",
        route: "bee-ai",
        bootstrapIcon: "bi-robot",
      },
      // {
      //   heading: "models.Test",
      //   route: "test",
      //   bootstrapIcon: "bi-layers",
      // },
    ],
  },
  // ===== CẤP 1: Truyền thông tiếp thị (YÊU CẦU: CHỈ 2 CẤP) =====
  {
    heading: "models.MarketingCommunications",
    route: "regal-edu",
    bootstrapIcon: "bi-camera-reels",
    pages: [
      { heading: "models.AffiliatePartner", route: "affiliate-partner", bootstrapIcon: "bi-person-check" },
      { heading: "models.Item", route: "item", bootstrapIcon: "bi-file-pdf" },
      { heading: "models.Event", route: "event", bootstrapIcon: "bi-calendar-event" },
      { heading: "models.EventAllocation", route: "event-allocation", bootstrapIcon: "bi-diagram-3" },
      { heading: "models.EventAllocationRegion", route: "event-allocation-regions", bootstrapIcon: "bi-people" },
      { heading: "models.EventAllocationCompany", route: "event-allocation-companies", bootstrapIcon: "bi-building" },
      { heading: "models.EventAllocationProposal", route: "event-allocation-proposals", bootstrapIcon: "bi-file-earmark-text" },
      { heading: "models.EventReport", route: "event-report", bootstrapIcon: "bi-card-list" },
      { heading: "models.LuckyDraw", route: "lucky-draw", bootstrapIcon: "bi-stars" },
      { heading: "models.Custom", route: "custome", bootstrapIcon: "bi-person-add" },

    ],
  },

  // ===== CẤP 1: Tuyển sinh (YÊU CẦU: CHỈ 2 CẤP) =====
  {
    heading: "models.Admissions",
    route: "regal-edu",
    bootstrapIcon: "bi-mortarboard",
    pages: [
      { heading: "models.AdmissionsQuota", route: "admissions-quota", bootstrapIcon: "bi-diagram-3" },
      { heading: "models.AdmissionsQuotaRegion", route: "admissions-quota-region", bootstrapIcon: "bi-building" },
      { heading: "models.AdmissionsQuotaCompany", route: "admissions-quota-company", bootstrapIcon: "bi-geo-alt" },
      { heading: "models.MyAdmissionsQuota", route: "my-admissions-quota", bootstrapIcon: "bi-people" },
      { heading: "models.Tuition", route: "tuition", bootstrapIcon: "bi-list-ul" },
      { heading: "models.Promotion", route: "promotion", bootstrapIcon: "bi-heart" },
      { heading: "models.CouponType", route: "coupon-type", bootstrapIcon: "bi-card-list" },
      { heading: "models.Coupon", route: "coupon", bootstrapIcon: "bi-percent" },
      { heading: "models.PromotionGroup", route: "promotion-group", bootstrapIcon: "bi-card-list" },
      { heading: "models.Gift", route: "gift", bootstrapIcon: "bi-gift" },
      { heading: "models.RegisterStudy", route: "register-study", bootstrapIcon: "bi-person-add" },
      { heading: "models.Receipt", route: "receipt", bootstrapIcon: "bi-receipt" },
      { heading: "models.OutputCommitment", route: "output-commitment", bootstrapIcon: "bi-check2-square" },

    ],
  },
  // ===== CẤP 1: Training (YÊU CẦU: CHỈ 2 CẤP) =====
  {
    heading: "models.Training",
    route: "regal-edu",
    bootstrapIcon: "bi-calendar-week",
    pages: [
      { heading: "models.LearningRoadMap", route: "learning-road-map", bootstrapIcon: "bi-map" },
      { heading: "models.Course", route: "course", bootstrapIcon: "bi-calendar-event" },
      { heading: "models.SupportingDocument", route: "supporting-document", bootstrapIcon: "bi-file-earmark-text" },
      // {
      //   heading: "models.Teacher",
      //   route: "teacher",
      //   bootstrapIcon: "bi-person-badge",
      // },
      // {
      //   heading: "models.Student",
      //   route: "student",
      //   keenthemesIcon: "user-square",
      //   bootstrapIcon: "bi-person",
      // },
      // {
      //   heading: "models.StudentProfile",
      //   route: "student-profile",
      //   keenthemesIcon: "user-square",
      //   bootstrapIcon: "bi-person",
      // },
      // {
      //   heading: "models.User",
      //   route: "application-user",
      //   keenthemesIcon: "user-square",
      //   bootstrapIcon: "bi-person",
      //   targetBlank: true,
      // },
      // {
      //   heading: "models.Class",
      //   route: "class",
      //   keenthemesIcon: "book-open",
      //   bootstrapIcon: "bi-journal-bookmark",
      // },


    ],
  },
  // ===== CẤP 1: Học vụ (YÊU CẦU: CHỈ 2 CẤP) =====
  {
    heading: "models.AcademicAffairs",
    route: "regal-edu",
    bootstrapIcon: "bi-journal-check",
    pages: [
      { heading: "models.Teacher", route: "teacher", bootstrapIcon: "bi-person-badge" },
      { heading: "models.Student", route: "student", bootstrapIcon: "bi-person" },
      { heading: "models.ClassScheduleOverview", route: "class-sessions", bootstrapIcon: "bi-person" },
      { heading: "models.HomeTeacherSessions", route: "home-teacher-sessions", bootstrapIcon: "bi-calendar2-check" },
      { heading: "models.TeacherSessions", route: "teacher-sessions", bootstrapIcon: "bi-calendar2-check" },
      // { heading: "models.StudentProfile", route: "student-profile", bootstrapIcon: "bi-person" },
      { heading: "models.Class", route: "class", bootstrapIcon: "bi-journal-bookmark" },
      { heading: "models.StudentProfile", route: "student-dashboard", bootstrapIcon: "bi-journal-bookmark" },
    ],
  },
  // ===== CẤP 1: Nhân sự (YÊU CẦU: CHỈ 2 CẤP) =====
  {
    heading: "models.HumanResources",
    route: "regal-edu",
    bootstrapIcon: "bi-calendar-week",
    pages: [
      { heading: "models.RecruitmentInfo", route: "recruitment-info", bootstrapIcon: "bi-info-circle" },
      { heading: "models.RecruitmentApply", route: "recruitment-apply", bootstrapIcon: "bi-person-check" },
    ],
  },
  // ===== CẤP 1: CatalogAdministration (3 cấp) =====
  {
    heading: "models.Catalogue",
    route: "regal-edu",
    bootstrapIcon: "bi-collection",
    pages: [
      {
        sectionTitle: "models.Training",
        route: "/admin/security",
        bootstrapIcon: "bi-shield-lock",
        sub: [
          { heading: "models.Skill", route: "skill", bootstrapIcon: "bi-clipboard-check" },
          { heading: "models.LectureType", route: "lecture-type", bootstrapIcon: "bi-book" },
          { heading: "models.PartnerType", route: "partner-type", bootstrapIcon: "bi-link-45deg" },
          { heading: "models.Degree", route: "degree", bootstrapIcon: "bi-mortarboard" },
          { heading: "models.ClassType", route: "class-type", bootstrapIcon: "bi-door-open" },
          { heading: "models.AgeGroup", route: "age-group", bootstrapIcon: "bi-people" },
          // { heading: "models.AccountGroup", route: "account-group", bootstrapIcon: "bi-person" },
          // { heading: "models.AccountGroupPermission", route: "account-group-permission", bootstrapIcon: "bi-journal-bookmark" },
        ],
      },
      {
        sectionTitle: "models.Organization",
        route: "/admin/org",
        bootstrapIcon: "bi-diagram-3",
        sub: [
          { heading: "models.Division", route: "division", bootstrapIcon: "bi-layers" },
          { heading: "models.Department", route: "department", bootstrapIcon: "bi-person-badge" },
          { heading: "models.Position", route: "position", bootstrapIcon: "bi-award" },
          { heading: "models.Region", route: "region", bootstrapIcon: "bi-globe" },
          { heading: "models.Company", route: "company", bootstrapIcon: "bi-geo-alt" },
          { heading: "models.Employee", route: "employee", bootstrapIcon: "bi-people" },
        ],
      },
      // {
      //   heading: "models.RegisterStudy",
      //   route: "register-study",
      //   bootstrapIcon: "bi-card-list",
      // },
    ],
  },

  // ===== CẤP 1: Hệ thống =====
  {
    heading: "models.System",
    route: "regal-edu",
    bootstrapIcon: "bi-gear",
    pages: [
      { heading: "models.AccountGroup", route: "account-group", bootstrapIcon: "bi-person-check" },
      { heading: "models.AccountGroupPermission", route: "account-group-permission", bootstrapIcon: "bi-file-pdf" },
      { heading: "models.WorkingTimeConfiguration", route: "working-time-configuration", bootstrapIcon: "bi-clock" },
      { heading: "models.HolidayType", route: "holiday-type", bootstrapIcon: "bi-calendar" },
    ],
  },
];

export default MainMenuConfig;
