import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from "vue-router";
//import { useAuthStore } from "@/stores/auth";
import { useAuthStore } from '@/stores/useAuthStore';
import { useConfigStore } from "@/stores/config";
import { useTabStore } from "@/stores/tabStore";
import { i18n } from "@/core/plugins/i18n";

const ignoreRoute: string[] = ['/account-group', '/account-group-permission', '/login', '/sign-in', '/sign-up', '/login-callback', '/bee-ai',
  '/no-privilege', '/confirm-account', '/page-not-found', '/404', '/500', '/password-reset', '/auth', '/dashboard', '/test'
];
import { userPermissionStore } from '@/stores/permissionStore';
const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    redirect: "/dashboard",
    component: () => import("@/layouts/default-layout/DefaultLayout.vue"),
    meta: {
      middleware: "auth",
    },
    children: [
      {
        path: "/dashboard",
        name: "dashboard",
        component: () => import("@/views/Dashboard.vue"),
        meta: {
          pageTitle: "Dashboard",
          breadcrumbs: ["Dashboards"],
        },
      },
      {
        path: "/bee-ai",
        name: "bee-ai",
        component: () => import("@/views/BeeAi.vue"),
        meta: {
          pageTitle: "beeAI",
          breadcrumbs: ["beeAI"],
        },
      },
      {
        path: '/test',
        name: 'test',
        component: () => import('@/views/TestToast.vue'),
        meta: {
          pageTitle: "TestToast",
          breadcrumbs: ["Toast"],
        },
      },
      {
        path: "/builder",
        name: "builder",
        component: () => import("@/views/LayoutBuilder.vue"),
        meta: {
          pageTitle: "Layout Builder",
          breadcrumbs: ["Layout"],
        },
      },
      {
        path: "/application-user",
        name: "application-user",
        component: () =>
          import("@/views/application-user/ApplicationUserList.vue"),
        meta: {
          pageTitle: "userManagement",
          breadcrumbs: ["Regal Edu", "userManagement"],
          formName: "application-user",
        },
      },

      {
        path: "/account-group",
        name: "account-group",
        component: () =>
          import("@/views/account-group/AccountGroupList.vue"),
        meta: {
          pageTitle: "AccountGroup",
          breadcrumbs: ["Regal Edu", "AccountGroup"],
          formName: "account-group",
        },
      },
      {
        path: "/account-group-permission",
        name: "account-group-permission",
        component: () =>
          import("@/views/account-group-permission/AccountGroupPermissionList.vue"),
        meta: {
          pageTitle: "AccountGroupPermission",
          breadcrumbs: ["Regal Edu", "AccountGroupPermission"],
          formName: "account-group-permission",
        },
      },
      {
        path: "/division",
        name: "division",
        component: () =>
          import("@/views/division/DivisionList.vue"),
        meta: {
          pageTitle: "Division",
          breadcrumbs: ["Regal Edu", "Division"],
          formName: "division",
        },
      },
      {
        path: "/department",
        name: "department",
        component: () =>
          import("@/views/department/DepartmentList.vue"),
        meta: {
          pageTitle: "Department",
          breadcrumbs: ["Regal Edu", "Department"],
          formName: "department",
        },
      },
      {
        path: "/event",
        name: "event",
        component: () =>
          import("@/views/event/EventList.vue"),
        meta: {
          pageTitle: "Event",
          breadcrumbs: ["Regal Edu", "Event"],
          formName: "event",
        },
      },
      {
        path: "/position",
        name: "position",
        component: () =>
          import("@/views/position/PositionList.vue"),
        meta: {
          pageTitle: "Position",
          breadcrumbs: ["Regal Edu", "Position"],
          formName: "position",
        },
      },
      {
        path: "/region",
        name: "region",
        component: () =>
          import("@/views/region/RegionList.vue"),
        meta: {
          pageTitle: "Region",
          breadcrumbs: ["Regal Edu", "Region"],
          formName: "region",
        },
      },
      {
        path: "/company",
        name: "company",
        component: () =>
          import("@/views/company/CompanyList.vue"),
        meta: {
          pageTitle: "Company",
          breadcrumbs: ["Regal Edu", "Company"],
          formName: "company",
        },
      },
      {
        path: "/employee",
        name: "employee",
        component: () =>
          import("@/views/employee/EmployeeList.vue"),
        meta: {
          pageTitle: "Employee",
          breadcrumbs: ["Regal Edu", "Employee"],
          formName: "employee",
        },
      },
      {
        path: "/working-time-configuration",
        name: "working-time-configuration",
        component: () =>
          import("@/views/working-time-configuration/WorkingTimeConfigurationList.vue"),
        meta: {
          pageTitle: "WorkingTimeConfiguration",
          breadcrumbs: ["Regal Edu", "WorkingTimeConfiguration"],
          formName: "working-time-configuration",
        },
      },
      {
        path: "/holiday-type",
        name: "holiday-type",
        component: () =>
          import("@/views/holiday-type/HolidayTypeList.vue"),
        meta: {
          pageTitle: "HolidayType",
          breadcrumbs: ["Regal Edu", "HolidayType"],
          formName: "holiday-type",
        },
      },
      {
        path: "/age-group",
        name: "age-group",
        component: () =>
          import("@/views/age-group/AgeGroupList.vue"),
        meta: {
          pageTitle: "AgeGroup",
          breadcrumbs: ["Regal Edu", "AgeGroup"],
          formName: "age-group",
        },
      },
      {
        path: "/skill",
        name: "skill",
        component: () =>
          import("@/views/skill/SkillList.vue"),
        meta: {
          pageTitle: "Skill",
          breadcrumbs: ["Regal Edu", "Skill"],
          formName: "skill",
        },
      },
      {
        path: "/item",
        name: "item",
        component: () =>
          import("@/views/item/ItemList.vue"),
        meta: {
          pageTitle: "Item",
          breadcrumbs: ["Regal Edu", "Item"],
          formName: "item",
        },
      },
      {
        path: "/class-type",
        name: "class-type",
        component: () =>
          import("@/views/class-type/ClassTypeList.vue"),
        meta: {
          pageTitle: "ClassType",
          breadcrumbs: ["Regal Edu", "ClassType"],
          formName: "class-type",
        },
      },
      {
        path: "/lecture-type",
        name: "lecture-type",
        component: () =>
          import("@/views/lecture-type/LectureTypeList.vue"),
        meta: {
          pageTitle: "LectureType",
          breadcrumbs: ["Regal Edu", "LectureType"],
          formName: "lecture-type",
        },
      },
      {
        path: "/partner-type",
        name: "partner-type",
        component: () =>
          import("@/views/partner-type/PartnerTypeList.vue"),
        meta: {
          pageTitle: "PartnerType",
          breadcrumbs: ["Regal Edu", "PartnerType"],
          formName: "partner-type",
        },
      },
      {
        path: "/affiliate-partner",
        name: "affiliate-partner",
        component: () =>
          import("@/views/affiliate-partner/AffiliatePartnerList.vue"),
        meta: {
          pageTitle: "AffiliatePartner",
          breadcrumbs: ["Regal Edu", "AffiliatePartner"],
          formName: "affiliate-partner",
        },
      },
      {
        path: "/degree",
        name: "degree",
        component: () =>
          import("@/views/degree/DegreeList.vue"),
        meta: {
          pageTitle: "Degree",
          breadcrumbs: ["Regal Edu", "Degree"],
          formName: "degree",
        },
      },
      {
        path: "/supporting-document",
        name: "supporting-document",
        component: () =>
          import("@/views/supporting-document/SupportingDocumentList.vue"),
        meta: {
          pageTitle: "SupportingDocument",
          breadcrumbs: ["Regal Edu", "SupportingDocument"],
          formName: "supporting-document",
        },
      },
      // {
      //   path: "/restore-data",
      //   name: "restore-data",
      //   component: () =>
      //     import("@/views/restore-data/RestoreList.vue"),
      //   meta: {
      //     pageTitle: "RestoreData",
      //     breadcrumbs: ["Regal Edu", "RestoreData"],
      //     formName: "restore-data",
      //   },
      // },
      {
        path: "/admissions-quota",
        name: "admissions-quota",
        component: () =>
          import("@/views/admissions-quota/AdmissionsQuotaList.vue"),
        meta: {
          pageTitle: "AdmissionsQuota",
          breadcrumbs: ["Regal Edu", "AdmissionsQuota"],
          formName: "admissions-quota",
        },
      },
      {
        path: "/my-admissions-quota",
        name: "my-admissions-quota",
        component: () =>
          import("@/views/admissions-quota/MyAdmissionsQuota.vue"),
        meta: {
          pageTitle: "MyAdmissionsQuota",
          breadcrumbs: ["Regal Edu", "MyAdmissionsQuota"],
          formName: "my-admissions-quota",
        },
      },
      {
        path: "/admissions-quota-region",
        name: "admissions-quota-region",
        component: () =>
          import("@/views/admissions-quota-region/AdmissionsQuotaRegionList.vue"),
        meta: {
          pageTitle: "AdmissionsQuotaRegion",
          breadcrumbs: ["Regal Edu", "AdmissionsQuotaRegion"],
          formName: "admissions-quota-region",
        },
      },
      {
        path: "/admissions-quota-company",
        name: "admissions-quota-company",
        component: () =>
          import("@/views/admissions-quota-company/AdmissionsQuotaCompanyList.vue"),
        meta: {
          pageTitle: "AdmissionsQuotaCompany",
          breadcrumbs: ["Regal Edu", "AdmissionsQuotaCompany"],
          formName: "admissions-quota-company",
        },
      },
      {
        path: "/tuition",
        name: "tuition",
        component: () =>
          import("@/views/tuition/TuitionList.vue"),
        meta: {
          pageTitle: "Tuition",
          breadcrumbs: ["Regal Edu", "Tuition"],
          formName: "tuition",
        },
      },
      {
        path: "/learning-road-map",
        name: "learning-road-map",
        component: () =>
          import("@/views/learning-road-map/LearningRoadMapList.vue"),
        meta: {
          pageTitle: "LearningRoadMap",
          breadcrumbs: ["Regal Edu", "LearningRoadMap"],
          formName: "learning-road-map",
        },
      },
      // Thiện
      {
        path: "/class",
        name: "class",
        component: () =>
          import("@/views/class/ClassList.vue"),
        meta: {
          pageTitle: "Class",
          breadcrumbs: ["Regal Edu", "Class"],
          formName: "class",
        },
      },
      {
        path: "/class-sessions",
        name: "class-sessions",
        component: () =>
          import("@/views/class-schedule/ClassScheduleOverview.vue"),
        meta: {
          pageTitle: "classScheduleOverview",
          breadcrumbs: ["Regal Edu", "Class Sessions"],
          formName: "class-sessions",
        },
      },
      {
        path: "/home-teacher-sessions",
        name: "home-teacher-sessions",
        component: () =>
          import("@/views/class-schedule/ClassScheduleHomeTeacherList.vue"),
        meta: {
          pageTitle: "HomeTeacherSessions",
          breadcrumbs: ["Regal Edu", "Home Teacher Sessions"],
          formName: "home-teacher-sessions",
        },
      },
      {
        path: "/teacher-sessions",
        name: "teacher-sessions",
        component: () =>
          import("@/views/teacher/TeacherSessionsList.vue"),
        meta: {
          pageTitle: "TeacherSessions",
          breadcrumbs: ["Regal Edu", "Teacher Sessions"],
          formName: "teacher-sessions",
        },
      },
      {
        path: "/teacher",
        name: "teacher",
        component: () =>
          import("@/views/teacher/TeacherList.vue"),
        meta: {
          pageTitle: "Teacher",
          breadcrumbs: ["Regal Edu", "Teacher"],
          formName: "teacher",
        },
      },
      {
        path: "/student",
        name: "student",
        component: () =>
          import("@/views/student/StudentList.vue"),
        meta: {
          pageTitle: "Student",
          breadcrumbs: ["Regal Edu", "Student"],
          formName: "student",
        },
      },
      {
        path: "/course",
        name: "course",
        component: () =>
          import("@/views/course/CourseList.vue"),
        meta: {
          pageTitle: "Course",
          breadcrumbs: ["Regal Edu", "Course"],
          formName: "course",
        },
      },
      {
        path: "/promotion",
        name: "promotion",
        component: () =>
          import("@/views/promotion/PromotionList.vue"),
        meta: {
          pageTitle: "promotion",
          breadcrumbs: ["Regal Edu", "promotion"],
          formName: "promotion",
        },
      },
      {
        path: "/coupon-type",
        name: "coupon-type",
        component: () =>
          import("@/views/coupon-type/CouponTypeList.vue"),
        meta: {
          pageTitle: "couponType",
          breadcrumbs: ["Regal Edu", "coupon-type"],
          formName: "coupon-type",
        },
      },
      {
        path: "/coupon",
        name: "coupon",
        component: () =>
          import("@/views/coupon-type/CouponList.vue"),
        meta: {
          pageTitle: "coupon",
          breadcrumbs: ["Regal Edu", "coupon"],
          formName: "coupon",
        },
      },
      {
        path: "/gift",
        name: "gift",
        component: () =>
          import("@/views/gift/GiftList.vue"),
        meta: {
          pageTitle: "gift",
          breadcrumbs: ["Regal Edu", "gift"],
          formName: "gift",
        },
      },
      {
        path: "/promotion-group",
        name: "promotion-group",
        component: () =>
          import("@/views/promotion-group/PromotionGroupList.vue"),
        meta: {
          pageTitle: "promotionGroup",
          breadcrumbs: ["Regal Edu", "promotionGroup"],
          formName: "promotion-group",
        },
      },
      {

        path: "/recruitment-apply",
        name: "recruitment-apply",
        component: () =>
          import("@/views/recruitment-apply/RecruitmentApplyList.vue"),
        meta: {
          pageTitle: "recruitmentApply",
          breadcrumbs: ["Regal Edu", "recruitmentApply"],
          formName: "recruitment-apply",
        },
      },
      {
        path: "/recruitment-info",
        name: "recruitment-info",
        component: () =>
          import("@/views/recruitment-info/RecruitmentInfoList.vue"),
        meta: {
          pageTitle: "recruitmentInfo",
          breadcrumbs: ["Regal Edu", "recruitmentInfo"],
          formName: "recruitment-info",
        },
      },
      {
        path: "/register-study",
        name: "register-study",
        component: () =>
          import("@/views/registerStudy/RegisterStudyList.vue"),
        meta: {
          pageTitle: "registerStudy",
          breadcrumbs: ["Regal Edu", "register-study"],
          formName: "register-study",
        },
      },
      // thêm route cho Receipt
      {
        path: "/receipt",
        name: "receipt",
        component: () =>
          import("@/views/receipt/ReceiptList.vue"),
      },
      {
        path: "/custome",
        name: "custome",
        component: () =>
          import("@/views/custome/CustomeList.vue"),
        meta: {
          pageTitle: "custome",
          breadcrumbs: ["Regal Edu", "custome"],
          formName: "custome",
        },
      },
      {
        path: "/event-allocation",
        name: "event-allocation",
        component: () =>
          import("@/views/event-allocation/EventAllocationList.vue"),
        meta: {
          pageTitle: "eventAllocation",
          breadcrumbs: ["Regal Edu", "event-allocation"],
          formName: "event-allocation",
        },
      },
      {
        path: "/event-allocation-regions",
        name: "event-allocation-regions",
        component: () =>
          import("@/views/event-allocation/EventAllocationRegionList.vue"),
        meta: {
          pageTitle: "eventAllocationRegion",
          breadcrumbs: ["Regal Edu", "event-allocation-regions"],
          formName: "event-allocation-regions",
        },
      },
      {
        path: "/event-allocation-companies",
        name: "event-allocation-companies",
        component: () =>
          import("@/views/event-allocation/EventAllocationCompanyList.vue"),
        meta: {
          pageTitle: "eventAllocationCompany",
          breadcrumbs: ["Regal Edu", "event-allocation-companies"],
          formName: "event-allocation-companies",
        },
      },
      {
        path: "/event-allocation-proposals",
        name: "event-allocation-proposals",
        component: () =>
          import("@/views/event-allocation/CompanyEventProposalList.vue"),
        meta: {
          pageTitle: "eventAllocationProposal",
          breadcrumbs: ["Regal Edu", "event-allocation-proposals"],
          formName: "event-allocation-proposals",
        },
      },
      {
        path: "/event-report",
        name: "event-report",
        component: () => import("@/views/event-allocation/EventReportList.vue"),
        meta: {
          pageTitle: "eventReport",
          breadcrumbs: ["Regal Edu", "event-report"],
          formName: "event-report",
        },
      },
      {
        path: "/output-commitment",
        name: "output-commitment",
        component: () =>
          import("@/views/output-commitment/OutputCommitmentList.vue"),
        meta: {
          pageTitle: "outputCommitment",
          breadcrumbs: ["Regal Edu", "output-commitment"],
          formName: "output-commitment",
        },
      },
    ],
  },
  {
    path: "/",
    component: () => import("@/layouts/AuthLayout.vue"),
    children: [
      {
        path: "/sign-in",
        name: "sign-in",
        component: () =>
          import("@/views/crafted/authentication/basic-flow/SignIn.vue"),
        meta: {
          pageTitle: "Sign In",
          noTab: true, // không mở tab cho trang đăng nhập
        },
      },
      // {
      //   path: "/sign-up",
      //   name: "sign-up",
      //   component: () =>
      //     import("@/views/crafted/authentication/basic-flow/SignUp.vue"),
      //   meta: {
      //     pageTitle: "Sign Up",
      //   },
      // },
      {
        path: "/password-reset",
        name: "password-reset",
        component: () =>
          import("@/views/crafted/authentication/basic-flow/PasswordReset.vue"),
        meta: {
          pageTitle: "Password reset",
          noTab: true, // không mở tab cho trang quên mật khẩu
        },
      },
    ],
  },
  {
    path: "/",
    component: () => import("@/layouts/SystemLayout.vue"),
    children: [
      {
        // the 404 route, when none of the above matches
        path: "/404",
        name: "404",
        component: () => import("@/views/crafted/authentication/Error404.vue"),
        meta: {
          pageTitle: "Error 404",
          noTab: true, // không mở tab cho trang 404
        },
      },
      {
        path: "/500",
        name: "500",
        component: () => import("@/views/crafted/authentication/Error500.vue"),
        meta: {
          pageTitle: "Error 500",
          noTab: true, // không mở tab cho trang 500
        },
      },
    ],
  },
  {
    path: "/:pathMatch(.*)*",
    redirect: "/404",
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to) {
    // If the route has a hash, scroll to the section with the specified ID; otherwise, scroll toc the top of the page.
    if (to.hash) {
      return {
        el: to.hash,
        top: 80,
        behavior: "smooth",
      };
    } else {
      return {
        top: 0,
        left: 0,
        behavior: "smooth",
      };
    }
  },
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const configStore = useConfigStore();
  const t = i18n.global.t as (key: string) => string;

  // 1. Bỏ qua các route không cần phân quyền
  if (ignoreRoute.includes(to.path)) {
    next();
    return;
  }

  // 2. Nếu là route yêu cầu đăng nhập
  const requireAuth = to.meta.middleware === 'auth' || to.matched.some(r => r.meta.middleware === 'auth');
  if (requireAuth && (!authStore.token || authStore.token === "")) {
    next({ name: 'sign-in', query: { returnUrl: to.fullPath } });
    return;
  }

  // 3. Kiểm tra phân quyền nếu đã có token
  if (authStore.token && authStore.token !== "") {
    const permissionRedirect = await loadMenuPermission(to);
    if (permissionRedirect) {
      next(permissionRedirect);
      return;
    }
  }

  // 4. Đặt title và reset layout
  if (to.meta.pageTitle) {
    const pageTitleKey =
      typeof to.meta.pageTitle === "string"
        ? to.meta.pageTitle
        : to.name?.toString() ?? to.fullPath;
    document.title = `${t('models.' + pageTitleKey)} - ${import.meta.env.VITE_APP_NAME}`;
  }
  configStore.resetLayoutConfig();
  const MAX_TABS = 6;
  const openedTabs = useTabStore().openedTabs;
  const isTabExist = openedTabs.some(tab => tab.route === to.path);

  if (!isTabExist && openedTabs.length >= MAX_TABS) {
    next(false);
    return;
  }

  // 5. Cho phép đi tiếp
  next();
});


async function loadMenuPermission(to) {
  if (ignoreRoute.includes(to.path)) {
    return null; // bỏ qua kiểm tra quyền
  }

  sessionStorage.setItem("form_active", to.name ?? "");

  const permissionStore = userPermissionStore();

  let hasError = false;
  // Kiểm tra xem đã tải resource chưa, nếu chưa thì tải
  if (!permissionStore.hasLoaded) {
    await permissionStore.loadResource().catch(() => {
      hasError = true;
    });
  }
  if (hasError) {
    return { path: '/page-not-found', query: { redirect: to.path } };
  }

  const listMenuAccept = permissionStore.getListMenuAccept || [];
  const pageName = to.name;

  if (!listMenuAccept.includes(pageName)) {
    return { path: '/no-privilege' };
  }

  return null;
}

router.afterEach((to) => {
  const tabStore = useTabStore();

  const title =
    typeof to.meta.pageTitle === "string"
      ? to.meta.pageTitle
      : to.name?.toString() || to.fullPath;

  const name = to.name?.toString() || to.fullPath;
  const route = to.path;

  // Không mở tab cho 404, auth và các trang không cần tab
  const authRoutes = ["/sign-in", "/sign-up", "/password-reset"];
  if (
    route !== "/404" &&
    route !== "/500" &&
    !route.startsWith("/auth") &&
    !authRoutes.includes(route)
  ) {
    const isTabExist = tabStore.openedTabs.some(tab => tab.route === route);
    if (!isTabExist) {
      tabStore.openTab({ name, route, title });
    } else {
      tabStore.setActiveTab(route);
    }
  }
})


export default router;
