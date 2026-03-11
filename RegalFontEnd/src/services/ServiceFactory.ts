// src/services/ServiceFactory.ts
import { apiFactory } from '@/plugins/ApiFactory';
import { ApplicationUserService } from './ApplicationUserService';
import { AccountGroupService } from './AccountGroupService';
import { AccountGroupPermissionService } from './accountGroupPermissionService';
import { AccountGroupEmployeeService } from './accountGroupEmployeeService';
import { DivisionService } from './DivisionService';
import { CommonService } from './CommonService';
import { DepartmentService } from './DepartmentService';
import { PositionService } from './PositionService';
import { RegionService } from './RegionService';
import { CompanyService } from './CompanyService';
import { EmployeeService } from './EmployeeService';
import { WorkingTimeService } from './WorkingTimeService';
import { HolidayService } from './HolidayService';
import { WorkingTimeConfigurationService } from './WorkingTimeConfigurationService';
import { HolidayTypeService } from './HolidayTypeService';
import { ClassTypeService } from './ClassTypeService';
import { AgeGroupService } from './AgeGroupService';
import { DegreeService } from './DegreeService';
import { LectureTypeService } from './LectureTypeService';
import { FileService } from './FileService';
import { SupportingDocumentService } from './SupportingDocumentService';
import { AdmissionsQuotaRegionService } from './AdmissionsQuotaRegionService';
import { AdmissionsQuotaService } from './AdmissionsQuotaService';
import { AdmissionsQuotaCompanyService } from './AdmissionsQuotaCompanyService';
import { TuitionService } from './TuitionService';
import { LearningRoadMapService } from './LearningRoadMapService';
import { CourseService } from './CourseService';
import { SkillService } from './SkillService';
import { ItemService } from './ItemService';
import { PartnerTypeService } from './PartnerTypeService';
import { AffiliatePartnerService } from './AffiliatePartnerService';
import { EventService } from './EventService';
import { ClassService } from './ClassService'; // Thiện
import { TeacherService } from './TeacherService';
import { StudentService } from './StudentService';
import { CompanyTeacherService } from './CompanyTeacherService';
import { AssignedClassService } from './AssignedClassService';
import { ClassScheduleService } from './ClassScheduleService';
import { ClassAttendantService } from './ClassAttendantService';
import { TeacherSessionService } from './TeacherSessionService';
import { EvaluateTeacherService } from './EvaluateTeacherService';

import { PromotionService } from './PromotionService';
import { GiftService } from './GiftService';
import { PromotionGroupService } from './PromotionGroupService';
import { CouponTypeService } from './CouponTypeService';
import { RegisterStudyService } from './RegisterStudyService';
import { ReceiptService } from './receiptService';
import { ClassScoreBoardService } from './ClassScoreBoardService';
//khai báo CouponIssueService
import { CouponIssueService } from './CouponIssueService';
import { LuckyDrawService } from './LuckyDrawService'; //Hùng
class ServiceFactory {

  private _applicationUserService?: ApplicationUserService;
  private _divisionService?: DivisionService;
  private _accountGroupService?: AccountGroupService;
  private _accountGroupPermissionService?: AccountGroupPermissionService; // Placeholder for AccountGroupPermissionService, if needed
  private _accountGroupEmployeeService?: AccountGroupEmployeeService; // Placeholder for AccountGroupEmployeeService, if needed
  private _commonService?: CommonService; // Placeholder for CommonService, if needed
  private _departmentService?: DepartmentService; // Placeholder for DepartmentService, if needed
  private _positionService?: PositionService; // Placeholder for PositionService, if needed
  private _regionService?: RegionService; // Placeholder for RegionService, if needed
  private _companyService?: CompanyService; // Placeholder for CompanyService, if needed
  private _employeeService?: EmployeeService; // Placeholder for EmployeeService, if needed
  private _workingTimeService?: WorkingTimeService; // Placeholder for WorkingTimeService, if needed
  private _holidayService?: HolidayService; // Placeholder for HolidayService, if needed
  private _workingTimeConfigurationService?: WorkingTimeConfigurationService; // Placeholder for WorkingTimeConfigurationService, if needed
  private _holidayTypeService?: HolidayTypeService; // Placeholder for HolidayTypeService, if needed
  private _classTypeService?: ClassTypeService;
  private _ageGroupService?: AgeGroupService; // Placeholder for AgeGroupService, if needed
  private _degreeService?: DegreeService; // Placeholder for DegreeService, if needed
  private _lectureTypeService?: LectureTypeService; // Placeholder for LectureTypeService, if needed
  private _fileService?: FileService; // Placeholder for FileService, if needed
  private _supportingDocumentService?: SupportingDocumentService; // Placeholder for SupportingDocumentService, if needed
  private _admissionsQuotaRegionService?: AdmissionsQuotaRegionService; // Placeholder for AdmissionsQuotaRegionService, if needed
  private _admissionsQuotaService?: AdmissionsQuotaService; // Placeholder for AdmissionsQuotaService, if needed
  private _admissionsQuotaCompanyService?: AdmissionsQuotaCompanyService; // Placeholder for AdmissionsQuotaCompanyService, if needed
  private _TuitionService?: TuitionService; // Placeholder for TuitionService, if needed
  private _learningRoadMapService?: LearningRoadMapService;
  private _courseService?: CourseService; // Placeholder for CourseService, if needed
  private _skillService?: SkillService; // Placeholder for SkillService, if needed
  private _itemService?: ItemService; // Placeholder for ItemService, if needed
  private _partnerTypeService?: PartnerTypeService; // Placeholder for PartnerTypeService, if needed
  private _affiliateService?: AffiliatePartnerService; // Placeholder for AffiliateService, if needed
  private _eventService?: EventService; // Placeholder for EventService, if needed
  private _classService?: ClassService; // Thiện | Thêm biến private
  private _teacherService?: TeacherService;
  private _studentService?: StudentService;
  private _companyTeacherService: CompanyTeacherService | null = null;
  private _assignedClassService?: AssignedClassService;
  private _classScheduleService?: ClassScheduleService;
  private _classAttendantService?: ClassAttendantService;
  private _teacherSessionService?: TeacherSessionService;
  private _evaluateTeacherService?: EvaluateTeacherService;
  private _promotionService?: PromotionService; // Placeholder for AdmissionsQuotaCompanyService, if needed
  private _giftService?: GiftService; // Placeholder for GiftService, if needed
  private _promotionGroupService?: PromotionGroupService; // Placeholder for PromotionGroupService, if needed
  private _couponTypeService?: CouponTypeService; // Placeholder for CouponTypeService, if needed

  public get eventService(): EventService {
    if (!this._eventService) {
      this._eventService = new EventService(apiFactory.eventApi);
    }
    return this._eventService;
  }
  public get affiliatePartnerService(): AffiliatePartnerService {
    if (!this._affiliateService) {
      this._affiliateService = new AffiliatePartnerService(apiFactory.affiliatePartnerApi);
    }
    return this._affiliateService;
  }
  public get partnerTypeService(): PartnerTypeService {
    if (!this._partnerTypeService) {
      this._partnerTypeService = new PartnerTypeService(apiFactory.partnerTypeApi);
    }
    return this._partnerTypeService;
  }
  public get itemService(): ItemService {
    if (!this._itemService) {
      this._itemService = new ItemService(apiFactory.itemApi);
    }
    return this._itemService;
  }


  // ✅ Method cho phép mock trong test
  public setMockApplicationUserService(service: ApplicationUserService) {
    this._applicationUserService = service;
  }

  public setMockAccountGroupService(service: AccountGroupService) {
    this._accountGroupService = service;
  }

  /**
   * Gets the instance of ApplicationUserService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */

  public get applicationUserService(): ApplicationUserService {
    if (!this._applicationUserService) {
      this._applicationUserService = new ApplicationUserService(apiFactory.applicationUserApi);
    }
    return this._applicationUserService;
  }
  /**
   * Gets the instance of AccountGroupService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get accountGroupService(): AccountGroupService {
    if (!this._accountGroupService) {
      this._accountGroupService = new AccountGroupService(apiFactory.accountGroupApi);
    }
    return this._accountGroupService;
  }
  /**
   * Gets the instance of AccountGroupPermissionService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get accountGroupPermissionService(): AccountGroupPermissionService {
    if (!this._accountGroupPermissionService) {
      this._accountGroupPermissionService = new AccountGroupPermissionService(apiFactory.accountGroupPermissionApi);
    }
    return this._accountGroupPermissionService;
  }
  /**
   * Gets the instance of AccountGroupEmployeeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get accountGroupEmployeeService(): AccountGroupEmployeeService {
    if (!this._accountGroupEmployeeService) {
      this._accountGroupEmployeeService = new AccountGroupEmployeeService(apiFactory.accountGroupEmployeeApi);
    }
    return this._accountGroupEmployeeService;
  }
  /**
   * Gets the instance of DivisionService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get divisionService(): DivisionService {
    if (!this._divisionService) {
      this._divisionService = new DivisionService(apiFactory.divisionApi);
    }
    return this._divisionService;
  }
  /**
   * Gets the instance of CommonService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get commonService(): CommonService {
    if (!this._commonService) {
      this._commonService = new CommonService(apiFactory.commonApi);
    }
    return this._commonService;
  }
  /**
   * Gets the instance of DepartmentService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get departmentService(): DepartmentService {
    if (!this._departmentService) {
      this._departmentService = new DepartmentService(apiFactory.departmentApi);
    }
    return this._departmentService;
  }
  /**
   * Gets the instance of PositionService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get positionService(): PositionService {
    if (!this._positionService) {
      this._positionService = new PositionService(apiFactory.positionApi);
    }
    return this._positionService;
  }
  /**
   * Gets the instance of RegionService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get regionService(): RegionService {
    if (!this._regionService) {
      this._regionService = new RegionService(apiFactory.regionApi);
    }
    return this._regionService;
  }
  /**
   * Gets the instance of CompanyService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get companyService(): CompanyService {
    if (!this._companyService) {
      this._companyService = new CompanyService(apiFactory.companyApi);
    }
    return this._companyService;
  }
  /**
   * Gets the instance of EmployeeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get employeeService(): EmployeeService {
    if (!this._employeeService) {
      this._employeeService = new EmployeeService(apiFactory.employeeApi);
    }
    return this._employeeService;
  }
  /**
   * Gets the instance of WorkingTimeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get workingTimeService(): WorkingTimeService {
    if (!this._workingTimeService) {
      this._workingTimeService = new WorkingTimeService(apiFactory.workingTimeApi);
    }
    return this._workingTimeService;
  }
  /**
   * Gets the instance of HolidayService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get holidayService(): HolidayService {
    if (!this._holidayService) {
      this._holidayService = new HolidayService(apiFactory.holidayApi);
    }
    return this._holidayService;
  }
  /**
   * Gets the instance of WorkingTimeConfigurationService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get workingTimeConfigurationService(): WorkingTimeConfigurationService {
    if (!this._workingTimeConfigurationService) {
      this._workingTimeConfigurationService = new WorkingTimeConfigurationService(apiFactory.workingTimeConfigurationApi);
    }
    return this._workingTimeConfigurationService;
  }
  /**
   * Gets the instance of HolidayTypeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get holidayTypeService(): HolidayTypeService {
    if (!this._holidayTypeService) {
      this._holidayTypeService = new HolidayTypeService(apiFactory.holidayTypeApi);
    }
    return this._holidayTypeService;
  }
  /**
   * Gets the instance of ClassTypeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get classTypeService(): ClassTypeService {
    if (!this._classTypeService) {
      this._classTypeService = new ClassTypeService(apiFactory.classTypeApi);
    }
    return this._classTypeService;
  }
  /**
   * Gets the instance of AgeGroupService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get ageGroupService(): AgeGroupService {
    if (!this._ageGroupService) {
      this._ageGroupService = new AgeGroupService(apiFactory.ageGroupApi);
    }
    return this._ageGroupService;
  }
  /**
   * Gets the instance of DegreeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get degreeService(): DegreeService {
    if (!this._degreeService) {
      this._degreeService = new DegreeService(apiFactory.degreeApi);
    }
    return this._degreeService;
  }
  /**
   * Gets the instance of LectureTypeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get lectureTypeService(): LectureTypeService {
    if (!this._lectureTypeService) {
      this._lectureTypeService = new LectureTypeService(apiFactory.lectureTypeApi);
    }
    return this._lectureTypeService;
  }
  /**
   * Gets the instance of FileService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get fileService(): FileService {
    if (!this._fileService) {
      this._fileService = new FileService(apiFactory.fileApi);
    }
    return this._fileService;
  }

  /**
   * Gets the instance of SupportingDocumentService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get supportingDocumentService(): SupportingDocumentService {
    if (!this._supportingDocumentService) {
      this._supportingDocumentService = new SupportingDocumentService(apiFactory.supportingDocumentApi);
    }
    return this._supportingDocumentService;
  }
  public get admissionsQuotaRegionService(): AdmissionsQuotaRegionService {
    if (!this._admissionsQuotaRegionService) {
      this._admissionsQuotaRegionService = new AdmissionsQuotaRegionService(apiFactory.admissionsQuotaRegionApi);
    }
    return this._admissionsQuotaRegionService;
  }
  public get admissionsQuotaService(): AdmissionsQuotaService {
    if (!this._admissionsQuotaService) {
      this._admissionsQuotaService = new AdmissionsQuotaService(apiFactory.admissionsQuotaApi);
    }
    return this._admissionsQuotaService;
  }
  public get admissionsQuotaCompanyService(): AdmissionsQuotaCompanyService {
    if (!this._admissionsQuotaCompanyService) {
      this._admissionsQuotaCompanyService = new AdmissionsQuotaCompanyService(apiFactory.admissionsQuotaCompanyApi);
    }
    return this._admissionsQuotaCompanyService;
  }
  public get TuitionService(): TuitionService {
    if (!this._TuitionService) {
      this._TuitionService = new TuitionService(apiFactory.TuitionApi);
    }
    return this._TuitionService;
  }
  public get learningRoadMapService(): LearningRoadMapService {
    if (!this._learningRoadMapService) {
      this._learningRoadMapService = new LearningRoadMapService(apiFactory.learningRoadMapApi);
    }
    return this._learningRoadMapService;
  }
  public get courseService(): CourseService {
    if (!this._courseService) {
      this._courseService = new CourseService(apiFactory.courseApi);
    }
    return this._courseService;
  }
  public get skillService(): SkillService {
    if (!this._skillService) {
      this._skillService = new SkillService(apiFactory.skillApi);
    }
    return this._skillService;
  }
  // Thiện
  /**
   * Gets the instance of ClassTypeService.
   * If it doesn't exist, it creates a new instance using the apiFactory.
   */
  public get classService(): ClassService {
    if (!this._classService) {
      this._classService = new ClassService(apiFactory.classApi);
    }
    return this._classService;
  }
  public get assignedClassService(): AssignedClassService {
    if (!this._assignedClassService) {
      this._assignedClassService = new AssignedClassService(apiFactory.assignedClassApi);
    }
    return this._assignedClassService;
  }
  public get classScheduleService(): ClassScheduleService {
    if (!this._classScheduleService) {
      this._classScheduleService = new ClassScheduleService(apiFactory.classScheduleApi);
    }
    return this._classScheduleService;
  }
  public get classAttendantService(): ClassAttendantService {
    if (!this._classAttendantService) {
      this._classAttendantService = new ClassAttendantService(apiFactory.classAttendantApi);
    }
    return this._classAttendantService;
  }
  public get teacherSessionService(): TeacherSessionService {
    if (!this._teacherSessionService) {
      this._teacherSessionService = new TeacherSessionService(apiFactory.teacherSessionApi);
    }
    return this._teacherSessionService;
  }
  public get evaluateTeacherService(): EvaluateTeacherService {
    if (!this._evaluateTeacherService) {
      this._evaluateTeacherService = new EvaluateTeacherService(apiFactory.evaluateTeacherApi);
    }
    return this._evaluateTeacherService;
  }
  public get teacherService(): TeacherService {
    if (!this._teacherService) {
      this._teacherService = new TeacherService(apiFactory.teacherApi);
    }
    return this._teacherService;
  }
  public get studentService(): StudentService {
    if (!this._studentService) {
      this._studentService = new StudentService(apiFactory.studentApi);
    }
    return this._studentService;
  }
  get companyTeacherService(): CompanyTeacherService {
    if (!this._companyTeacherService) {
      this._companyTeacherService = new CompanyTeacherService(apiFactory.companyTeacherApi);
    }
    return this._companyTeacherService;
  }
  public get promotionService(): PromotionService {
    if (!this._promotionService) {
      this._promotionService = new PromotionService(apiFactory.promotionApi);
    }
    return this._promotionService;
  }
  public get giftService(): GiftService {
    if (!this._giftService) {
      this._giftService = new GiftService(apiFactory.giftApi);
    }
    return this._giftService;
  }
  public get promotionGroupService(): PromotionGroupService {
    if (!this._promotionGroupService) {
      this._promotionGroupService = new PromotionGroupService(apiFactory.promotionGroupApi);

    }
    return this._promotionGroupService;
  }
  public get couponTypeService(): CouponTypeService {
    if (!this._couponTypeService) {
      this._couponTypeService = new CouponTypeService(apiFactory.couponTypeApi);
    }
    return this._couponTypeService;
  }
  //thêm khai báo RegisterStudyService
  private _registerStudyService?: RegisterStudyService;
  public get registerStudyService(): RegisterStudyService {
    if (!this._registerStudyService) {
      this._registerStudyService = new RegisterStudyService(apiFactory.registerStudyApi);
    }
    return this._registerStudyService;
  }
  //thêm khai báo cho ReceiptService
  private _receiptService?: ReceiptService;
  private _classScoreBoardService?: ClassScoreBoardService;

  public get receiptService(): ReceiptService {
    if (!this._receiptService) {
      this._receiptService = new ReceiptService(apiFactory.receiptApi);
    }
    return this._receiptService;
  }
  public get classScoreBoardService(): ClassScoreBoardService {
    if (!this._classScoreBoardService) {
      this._classScoreBoardService = new ClassScoreBoardService(apiFactory.classScoreBoardApi);
    }
    return this._classScoreBoardService;
  }


  //khai báo service CouponIssueService
  private _couponIssueService?: CouponIssueService;// Placeholder for CouponIssueService, if needed
  public get couponIssueService(): CouponIssueService {
    if (!this._couponIssueService) {
      this._couponIssueService = new CouponIssueService(apiFactory.couponIssueApi);
    }
    return this._couponIssueService;
  }

  private _luckyDrawService?: LuckyDrawService;
  public get luckyDrawService(): LuckyDrawService {
    if (!this._luckyDrawService) {
      this._luckyDrawService = new LuckyDrawService(apiFactory.luckyDrawApi);
    }
    return this._luckyDrawService;
  }


}




export const serviceFactory = new ServiceFactory();
