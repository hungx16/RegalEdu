import type {
  AssignedClassApi,
  AutoAssignStudentsCommand,
  ManualAssignStudentCommand,
  ManualUnassignStudentCommand
} from '@/api/AssignedClassApi';
import type { Result } from '@/types/Result';
import type { StudentModel } from '@/api/StudentApi';
import { useNotificationStore } from '@/stores/notificationStore';

export class AssignedClassService {
  private api: AssignedClassApi;

  constructor(apiInstance: AssignedClassApi) {
    this.api = apiInstance;
  }

  async fetchAssignableStudents(classId: string): Promise<Result<StudentModel[]>> {
    return await this.api.getAssignableStudents(classId);
  }

  async fetchAssignedStudents(classId: string): Promise<Result<StudentModel[]>> {
    return await this.api.getAssignedStudentsByClassId(classId);
  }

  async manualAssign(command: ManualAssignStudentCommand): Promise<Result<StudentModel>> {
    const result: any = await this.api.manualAssignStudent(command);
    if (!result.succeeded) {
      throw new Error(result.error || 'Assign failed');
    }
    return result;
  }

  async manualUnassign(command: ManualUnassignStudentCommand): Promise<Result<StudentModel>> {
    const result: any = await this.api.manualUnassignStudent(command);
    if (!result.succeeded) {
      throw new Error(result.error || 'Unassign failed');
    }
    return result;
  }

  async autoAssign(command: AutoAssignStudentsCommand): Promise<Result<StudentModel[]>> {
    const result: any = await this.api.autoAssignStudents(command);
    if (!result.succeeded) {
      throw new Error(result.error || 'Auto-assign failed');
    } else {
      useNotificationStore().showToast('success', { key: 'class.autoAssignSuccess' })
    }
    return result;
  }
}
