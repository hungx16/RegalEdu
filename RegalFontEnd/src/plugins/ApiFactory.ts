// src/plugins/ApiFactory.ts
import { AccountGroupApi } from '@/api/AccountGroupApi';
import { AccountGroupEmployeeApi } from '@/api/accountGroupEmployeeApi';
import { AccountGroupPermissionApi } from '@/api/accountGroupPermissionApi';
import { AgeGroupApi } from '@/api/AgeGroupApi';
import { ApplicationUserApi } from '@/api/ApplicationUserApi';
import { ClassTypeApi } from '@/api/ClassTypeApi';
import { CommonApi } from '@/api/CommonApi';
import { CompanyApi } from '@/api/CompanyApi';
import { DegreeApi } from '@/api/DegreeApi';
import { DepartmentApi } from '@/api/DepartmentApi';
import { DivisionApi } from '@/api/DivisionApi';
import { EmployeeApi } from '@/api/EmployeeApi';
import { FileApi } from '@/api/FileApi';
import { HolidayApi } from '@/api/HolidayApi';
import { HolidayTypeApi } from '@/api/HolidayTypeApi';
import { LectureTypeApi } from '@/api/LectureTypeApi';
import { PositionApi } from '@/api/PositionApi';
import { RegionApi } from '@/api/RegionApi';
import { WorkingTimeApi } from '@/api/WorkingTimeApi';
import { WorkingTimeConfigurationApi } from '@/api/WorkingTimeConfigurationApi';
import { SupportingDocumentApi } from '@/api/SupportingDocumentApi';
import { AdmissionsQuotaCompanyApi } from '@/api/AdmissionsQuotaCompanyApi';
import { AdmissionsQuotaApi } from '@/api/AdmissionsQuotaApi';
import { AdmissionsQuotaRegionApi } from '@/api/AdmissionsQuotaRegionApi';
import { TuitionApi } from '@/api/TuitionApi';
import { LearningRoadMapApi } from '@/api/LearningRoadMapApi';
import { CourseApi } from '@/api/CourseApi';
import { SkillApi } from '@/api/SkillApi';
import { ItemApi } from '@/api/ItemApi';
import { PartnerTypeApi } from '@/api/PartnerTypeApi';
import { AffiliatePartnerApi } from '@/api/AffiliatePartnerApi';
import { EventApi } from '@/api/EventApi';
import { ClassApi } from '@/api/ClassApi'; // Thiện
import { TeacherApi } from '@/api/TeacherApi';
import { StudentApi } from '@/api/StudentApi';
import { CompanyTeacherApi } from '@/api/CompanyTeacherApi';
import { AssignedClassApi } from '@/api/AssignedClassApi';
import { ClassScheduleApi } from '@/api/ClassScheduleApi';
import { ClassAttendantApi } from '@/api/ClassAttendantApi';
import { TeacherSessionApi } from '@/api/TeacherSessionApi';
import { EvaluateTeacherApi } from '@/api/EvaluateTeacherApi';

import { PromotionApi } from '@/api/PromotionApi';
import { GiftApi } from '@/api/GiftApi';
import { PromotionGroupApi } from '@/api/PromotionGroupApi';
import { CouponTypeApi } from '@/api/CouponTypeApi';
import { RegisterStudyApi } from '@/api/RegisterStudyApi';
import { ReceiptApi } from '@/api/ReceiptApi';
import { ClassScoreBoardApi } from '@/api/ClassScoreBoardApi';
import { CouponIssueApi } from '@/api/CouponIssueApi';
import { LuckyDrawApi } from '@/api/LuckyDrawApi'; //Hùng
class ApiFactory {

  private _applicationUserApi?: ApplicationUserApi;
  private _accountGroupApi?: AccountGroupApi; // Placeholder for AccountGroupApi, if needed
  private _accountGroupPermissionApi?: AccountGroupPermissionApi; // Placeholder for AccountGroupPermissionApi, if needed
  private _accountGroupEmployeeApi?: AccountGroupEmployeeApi; // Placeholder for AccountGroupEmployeeApi, if needed
  private _divisionApi?: DivisionApi; // Placeholder for DivisionApi, if needed
  private _commonApi?: CommonApi; // Placeholder for CommonApi, if needed
  private _departmentApi?: DepartmentApi; // Placeholder for DepartmentApi, if needed
  private _positionApi?: PositionApi; // Placeholder for PositionApi, if needed
  private _regionApi?: RegionApi; // Placeholder for RegionApi, if needed
  private _companyApi?: CompanyApi; // Placeholder for CompanyApi, if needed
  private _employeeApi?: EmployeeApi; // Placeholder for EmployeeApi, if needed
  private _workingTimeApi?: WorkingTimeApi; // Placeholder for WorkingTimeApi, if needed
  private _holidayApi?: HolidayApi; // Placeholder for HolidayApi, if needed
  private _workingTimeConfigurationApi?: WorkingTimeConfigurationApi; // Placeholder for WorkingTimeConfigurationApi, if needed
  private _holidayTypeApi?: HolidayTypeApi; // Placeholder for HolidayTypeApi, if needed
  private _classTypeApi?: ClassTypeApi; // Placeholder for ClassTypeApi, if needed
  private _ageGroupApi?: AgeGroupApi; // Placeholder for AgeGroupApi, if needed
  private _degreeApi?: DegreeApi; // Placeholder for DegreeApi, if needed
  private _lectureTypeApi?: LectureTypeApi; // Placeholder for LectureTypeApi, if needed
  private _fileApi?: FileApi; // Placeholder for FileApi, if needed
  private _supportingDocumentApi?: SupportingDocumentApi; // Placeholder for SupportingDocumentApi, if needed
  private _admissionsQuotaRegionApi?: AdmissionsQuotaRegionApi; // Placeholder for AdmissionsQuotaRegionApi, if needed
  private _admissionsQuotaApi?: AdmissionsQuotaApi; // Placeholder for AdmissionsQuotaApi, if needed
  private _admissionsQuotaCompanyApi?: AdmissionsQuotaCompanyApi; // Placeholder for AdmissionsQuotaCompanyApi, if needed
  private _learningRoadMapApi?: LearningRoadMapApi; // Placeholder for LearningRoadMapApi, if needed
  private _courseApi?: CourseApi; // Placeholder for CourseApi, if needed
  private _skillApi?: SkillApi; // Placeholder for SkillApi, if needed
  private _itemApi?: ItemApi; // Placeholder for ItemApi, if needed
  private _partnerTypeApi?: PartnerTypeApi; // Placeholder for PartnerTypeApi, if needed
  private _affiliatePartnerApi?: AffiliatePartnerApi; // Placeholder for AffiliatePartnerApi, if needed
  private _eventApi?: EventApi; // Placeholder for EventApi, if needed
  private _classApi?: ClassApi; // Thiện
  private _teacherApi?: TeacherApi; // Thien
  private _studentApi?: StudentApi; // Thien
  private _companyTeacherApi?: CompanyTeacherApi; // Thien
  private _assignedClassApi?: AssignedClassApi;
  private _classScheduleApi?: ClassScheduleApi;
  private _classAttendantApi?: ClassAttendantApi;
  private _teacherSessionApi?: TeacherSessionApi;
  private _evaluateTeacherApi?: EvaluateTeacherApi;
  private _promotionApi?: PromotionApi; // Placeholder for PromotionApi, if needed
  private _giftApi?: GiftApi; // Placeholder for GiftApi, if needed
  private _promotionGroupApi?: PromotionGroupApi; // Placeholder for PromotionGroupApi, if needed
  private _couponTypeApi?: CouponTypeApi;
  //khai báo RegisterStudyApi
  private _registerStudyApi?: RegisterStudyApi; // Placeholder for RegisterStudyApi, if needed
  private _receiptApi?: ReceiptApi; // Placeholder for ReceiptApi, if needed
  private _classScoreBoardApi?: ClassScoreBoardApi;
  //khai báo CouponIssueApi
  private _couponIssueApi?: CouponIssueApi; // Placeholder for CouponIssueApi, if needed

  private _luckyDrawApi?: LuckyDrawApi;

  /**
   * Gets the instance of LuckyDrawApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get eventApi(): EventApi {
    if (!this._eventApi) {
      this._eventApi = new EventApi();
    }
    return this._eventApi;
  }
  /**
   * Gets the instance of AffiliatePartnerApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get affiliatePartnerApi(): AffiliatePartnerApi {
    if (!this._affiliatePartnerApi) {
      this._affiliatePartnerApi = new AffiliatePartnerApi();
    }
    return this._affiliatePartnerApi;
  }
  /**
   * Gets the instance of PartnerTypeApi.
   * If it doesn't exist, it creates a new instance.
   * */
  public get partnerTypeApi(): PartnerTypeApi {
    if (!this._partnerTypeApi) {
      this._partnerTypeApi = new PartnerTypeApi();
    }
    return this._partnerTypeApi;
  }
  public get itemApi(): ItemApi {
    if (!this._itemApi) {
      this._itemApi = new ItemApi();
    }
    return this._itemApi;
  }

  /**
   * Gets the instance of SkillApi.
   * If it doesn't exist, it creates a new instance.
   */

  public get skillApi(): SkillApi {
    if (!this._skillApi) {
      this._skillApi = new SkillApi();
    }
    return this._skillApi;
  }
  /**
   * Gets the instance of ApplicationUserApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get applicationUserApi(): ApplicationUserApi {
    if (!this._applicationUserApi) {
      this._applicationUserApi = new ApplicationUserApi();
    }
    return this._applicationUserApi;
  }

  /**
   * Gets the instance of AccountGroupApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get accountGroupApi(): AccountGroupApi {
    if (!this._accountGroupApi) {
      this._accountGroupApi = new AccountGroupApi();
    }
    return this._accountGroupApi;
  }
  /**
   * Gets the instance of AccountGroupPermissionApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get accountGroupPermissionApi(): AccountGroupPermissionApi {
    if (!this._accountGroupPermissionApi) {
      this._accountGroupPermissionApi = new AccountGroupPermissionApi();
    }
    return this._accountGroupPermissionApi;
  }
  /**
   * Gets the instance of AccountGroupEmployeeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get accountGroupEmployeeApi(): AccountGroupEmployeeApi {
    if (!this._accountGroupEmployeeApi) {
      this._accountGroupEmployeeApi = new AccountGroupEmployeeApi();
    }
    return this._accountGroupEmployeeApi;
  }
  /**
   * Gets the instance of DivisionApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get divisionApi(): DivisionApi {
    if (!this._divisionApi) {
      this._divisionApi = new DivisionApi();
    }
    return this._divisionApi;
  }
  /**
   * Gets the instance of CommonApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get commonApi(): CommonApi {
    if (!this._commonApi) {
      this._commonApi = new CommonApi();
    }
    return this._commonApi;
  }
  /**
   * Gets the instance of DepartmentApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get departmentApi(): DepartmentApi {
    if (!this._departmentApi) {
      this._departmentApi = new DepartmentApi();
    }
    return this._departmentApi;
  }
  /**
   * Gets the instance of PositionApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get positionApi(): PositionApi {
    if (!this._positionApi) {
      this._positionApi = new PositionApi();
    }
    return this._positionApi;
  }
  /**
   * Gets the instance of RegionApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get regionApi(): RegionApi {
    if (!this._regionApi) {
      this._regionApi = new RegionApi();
    }
    return this._regionApi;
  }
  /**
   * Gets the instance of CompanyApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get companyApi(): CompanyApi {
    if (!this._companyApi) {
      this._companyApi = new CompanyApi();
    }
    return this._companyApi;
  }
  /**
   * Gets the instance of EmployeeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get employeeApi(): EmployeeApi {
    if (!this._employeeApi) {
      this._employeeApi = new EmployeeApi();
    }
    return this._employeeApi;
  }
  /**
   * Gets the instance of WorkingTimeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get workingTimeApi(): WorkingTimeApi {
    if (!this._workingTimeApi) {
      this._workingTimeApi = new WorkingTimeApi();
    }
    return this._workingTimeApi;
  }
  /**
   * Gets the instance of HolidayApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get holidayApi(): HolidayApi {
    if (!this._holidayApi) {
      this._holidayApi = new HolidayApi();
    }
    return this._holidayApi;
  }
  /**
   * Gets the instance of WorkingTimeConfigurationApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get workingTimeConfigurationApi(): WorkingTimeConfigurationApi {
    if (!this._workingTimeConfigurationApi) {
      this._workingTimeConfigurationApi = new WorkingTimeConfigurationApi();
    }
    return this._workingTimeConfigurationApi;
  }
  /**
   * Gets the instance of HolidayTypeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get holidayTypeApi(): HolidayTypeApi {
    if (!this._holidayTypeApi) {
      this._holidayTypeApi = new HolidayTypeApi();
    }
    return this._holidayTypeApi;
  }
  /**
   * Gets the instance of ClassTypeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get classTypeApi(): ClassTypeApi {
    if (!this._classTypeApi) {
      this._classTypeApi = new ClassTypeApi();
    }
    return this._classTypeApi;
  }
  /**
   * Gets the instance of AgeGroupApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get ageGroupApi(): AgeGroupApi {
    if (!this._ageGroupApi) {
      this._ageGroupApi = new AgeGroupApi();
    }
    return this._ageGroupApi;
  }
  /**
   * Gets the instance of DegreeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get degreeApi(): DegreeApi {
    if (!this._degreeApi) {
      this._degreeApi = new DegreeApi();
    }
    return this._degreeApi;
  }
  /**
   * Gets the instance of LectureTypeApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get lectureTypeApi(): LectureTypeApi {
    if (!this._lectureTypeApi) {
      this._lectureTypeApi = new LectureTypeApi();
    }
    return this._lectureTypeApi;
  }
  /**
   * Gets the instance of FileApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get fileApi(): FileApi {
    if (!this._fileApi) {
      this._fileApi = new FileApi();
    }
    return this._fileApi;
  }
  /**
   * Gets the instance of SupportingDocumentApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get supportingDocumentApi(): SupportingDocumentApi {
    if (!this._supportingDocumentApi) {
      this._supportingDocumentApi = new SupportingDocumentApi();
    }
    return this._supportingDocumentApi;
  }
  public get admissionsQuotaRegionApi(): AdmissionsQuotaRegionApi {
    if (!this._admissionsQuotaRegionApi) {
      this._admissionsQuotaRegionApi = new AdmissionsQuotaRegionApi();
    }
    return this._admissionsQuotaRegionApi;
  }
  public get admissionsQuotaApi(): AdmissionsQuotaApi {
    if (!this._admissionsQuotaApi) {
      this._admissionsQuotaApi = new AdmissionsQuotaApi();
    }
    return this._admissionsQuotaApi;
  }
  public get admissionsQuotaCompanyApi(): AdmissionsQuotaCompanyApi {
    if (!this._admissionsQuotaCompanyApi) {
      this._admissionsQuotaCompanyApi = new AdmissionsQuotaCompanyApi();
    }
    return this._admissionsQuotaCompanyApi;
  }
  private _TuitionApi?: TuitionApi; // Placeholder for TuitionApi, if needed
  /**
   * Gets the instance of TuitionApi.
   * If it doesn't exist, it creates a new instance.
   */
  public get TuitionApi(): TuitionApi {
    if (!this._TuitionApi) {
      this._TuitionApi = new TuitionApi();
    }
    return this._TuitionApi;
  }
  /**
   * Gets the instance of LearningRoadmapApi. 
   * If it doesn't exist, it creates a new instance.
   * Note: Using 'any' type as a placeholder. Replace 'any' with the actual type when available.
   */
  public get learningRoadMapApi(): LearningRoadMapApi {
    if (!this._learningRoadMapApi) {
      this._learningRoadMapApi = new LearningRoadMapApi();
    }
    return this._learningRoadMapApi;
  }
  /**
   * Gets the instance of CourseApi.
   * If it doesn't exist, it creates a new instance.
   * Note: Using 'any' type as a placeholder. Replace 'any' with the actual type when available.
   */
  public get courseApi(): CourseApi {
    if (!this._courseApi) {
      this._courseApi = new CourseApi();
    }
    return this._courseApi;
  }
  /**
   * Gets the instance of ClassApi.
   * If it doesn't exist, it creates a new instance.
   * Note: Using 'any' type as a placeholder. Replace 'any' with the actual type when available.
   */
  public get classApi(): ClassApi {
    if (!this._classApi) {
      this._classApi = new ClassApi();
    }
    return this._classApi;
  }
  public get assignedClassApi(): AssignedClassApi {
    if (!this._assignedClassApi) {
      this._assignedClassApi = new AssignedClassApi();
    }
    return this._assignedClassApi;
  }
  public get classScheduleApi(): ClassScheduleApi {
    if (!this._classScheduleApi) {
      this._classScheduleApi = new ClassScheduleApi();
    }
    return this._classScheduleApi;
  }
  public get classAttendantApi(): ClassAttendantApi {
    if (!this._classAttendantApi) {
      this._classAttendantApi = new ClassAttendantApi();
    }
    return this._classAttendantApi;
  }
  public get teacherSessionApi(): TeacherSessionApi {
    if (!this._teacherSessionApi) {
      this._teacherSessionApi = new TeacherSessionApi();
    }
    return this._teacherSessionApi;
  }
  public get evaluateTeacherApi(): EvaluateTeacherApi {
    if (!this._evaluateTeacherApi) {
      this._evaluateTeacherApi = new EvaluateTeacherApi();
    }
    return this._evaluateTeacherApi;
  }
  public get teacherApi(): TeacherApi {
    if (!this._teacherApi) {
      this._teacherApi = new TeacherApi();
    }
    return this._teacherApi;
  }
  public get studentApi(): StudentApi {
    if (!this._studentApi) {
      this._studentApi = new StudentApi();
    }
    return this._studentApi;
  }
  public get companyTeacherApi(): CompanyTeacherApi {
    if (!this._companyTeacherApi) {
      this._companyTeacherApi = new CompanyTeacherApi();
    }
    return this._companyTeacherApi;
  }
  public get promotionApi(): PromotionApi {
    if (!this._promotionApi) {
      this._promotionApi = new PromotionApi();
    }
    return this._promotionApi;
  }
  public get giftApi(): GiftApi {
    if (!this._giftApi) {
      this._giftApi = new GiftApi();
    }
    return this._giftApi;
  }
  public get promotionGroupApi(): PromotionGroupApi {
    if (!this._promotionGroupApi) {
      this._promotionGroupApi = new PromotionGroupApi();
    }
    return this._promotionGroupApi;
  }
  public get couponTypeApi(): CouponTypeApi {
    if (!this._couponTypeApi) {
      this._couponTypeApi = new CouponTypeApi();
    }
    return this._couponTypeApi;
  }
  //khai báo RegisterStudyApi
  public get registerStudyApi(): RegisterStudyApi {
    if (!this._registerStudyApi) {
      this._registerStudyApi = new RegisterStudyApi();
    }
    return this._registerStudyApi;
  }
  //khai báo cho ReceiptApi

  public get receiptApi(): ReceiptApi {
    if (!this._receiptApi) {
      this._receiptApi = new ReceiptApi();
    }
    return this._receiptApi;
  }
  public get classScoreBoardApi(): ClassScoreBoardApi {
    if (!this._classScoreBoardApi) {
      this._classScoreBoardApi = new ClassScoreBoardApi();
    }
    return this._classScoreBoardApi;
  }
  //khai báo CouponIssueApi
  public get couponIssueApi(): CouponIssueApi {
    if (!this._couponIssueApi) {
      this._couponIssueApi = new CouponIssueApi();
    }
    return this._couponIssueApi;
  }
  public get luckyDrawApi(): LuckyDrawApi {
    if (!this._luckyDrawApi) {
      this._luckyDrawApi = new LuckyDrawApi();
    }
    return this._luckyDrawApi;
  }

}
export const apiFactory = new ApiFactory();
