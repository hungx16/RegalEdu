import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { StudentModel } from '@/api/StudentApi';
import type {
  ManualAssignStudentCommand,
  ManualUnassignStudentCommand
} from '@/api/AssignedClassApi';

export const useAssignedClassStore = defineStore('assignedClass', {
  state: () => ({
    assignableStudents: [] as StudentModel[],
    assignedStudents: [] as StudentModel[],
    loading: false,
    assigning: false
  }),
  actions: {
    async fetchAssignableStudents(classId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.assignedClassService.fetchAssignableStudents(classId);
        this.assignableStudents = res.data || [];
        return res;
      } catch (error) {
        console.error('Error fetching assignable students:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAssignedStudents(classId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.assignedClassService.fetchAssignedStudents(classId);
        this.assignedStudents = res.data || [];
        return res;
      } catch (error) {
        console.error('Error fetching assigned students:', error);
      } finally {
        this.loading = false;
      }
    },
    async manualAssign(command: ManualAssignStudentCommand) {
      this.assigning = true;
      try {
        const res = await serviceFactory.assignedClassService.manualAssign(command);
        const student = res.data;
        if (student?.id) {
          const idx = this.assignedStudents.findIndex((s) => s.id === student.id);
          if (idx >= 0) {
            this.assignedStudents.splice(idx, 1, student);
          } else {
            this.assignedStudents.push(student);
          }
        }
        return res;
      } catch (error) {
        console.error('Error assigning student:', error);
      } finally {
        this.assigning = false;
      }
    },
    async manualUnassign(command: ManualUnassignStudentCommand) {
      this.assigning = true;
      try {
        const res = await serviceFactory.assignedClassService.manualUnassign(command);
        if (command.studentId) {
          this.assignedStudents = this.assignedStudents.filter((s) => s.id !== command.studentId);
        }
        return res;
      } catch (error) {
        console.error('Error unassigning student:', error);
      } finally {
        this.assigning = false;
      }
    },
    async autoAssign(classId: string) {
      this.assigning = true;
      try {
        const res = await serviceFactory.assignedClassService.autoAssign({ classId });
        if (res.data) {
          this.assignedStudents = res.data;
        } else {
          await this.fetchAssignedStudents(classId);
        }
        return res;
      } catch (error) {
        console.error('Error auto-assigning students:', error);
      } finally {
        this.assigning = false;
      }
    }
  }
});
