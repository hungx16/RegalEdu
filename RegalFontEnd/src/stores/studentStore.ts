// src/stores/studentStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { StudentModel, StudentQuery } from '@/api/StudentApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useStudentStore = defineStore('student', {
  state: () => ({
    students: [] as StudentModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as StudentQuery,
    selectedStudent: null as StudentModel | null,
  }),

  actions: {
    /** Lấy danh sách học viên có phân trang */
    async fetchPagedStudents() {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.fetchPagedStudents(this.query);
        if (result?.succeeded === true) {
          this.students = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching students:', error);
      } finally {
        this.loading = false;
      }
    },
    /** Lấy danh sách học viên có phân trang */
    async fetchPagedCustoms() {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.fetchPagedCustoms(this.query);
        if (result?.succeeded === true) {
          this.students = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching students:', error);
      } finally {
        this.loading = false;
      }
    },
    /** Lấy toàn bộ học viên (dùng cho table không phân trang) */
    async fetchAllCustoms() {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.fetchAllCustoms();
        if (result?.succeeded === true) {
          this.students = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching students:', error);
      } finally {
        this.loading = false;
      }
    },
    searchStudentOrParent(keyword: string) {
      const lowerKeyword = keyword.toLowerCase();
      return this.students.filter(student =>
        (student.fullName && student.fullName.toLowerCase().includes(lowerKeyword)) ||
        (student.phone && student.phone.includes(keyword)) ||
        (student.contacts?.some(contact => (contact.fullName ?? '').toLowerCase().includes(lowerKeyword)) ?? false) ||
        (student.contacts?.some(contact => (contact.phone ?? '').toLowerCase().includes(lowerKeyword)) ?? false)
      );
    },
    /** Lấy toàn bộ học viên (dùng cho table không phân trang) */
    async fetchAllStudents() {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.fetchAllStudents();
        if (result?.succeeded === true) {
          this.students = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching students:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchStudentDetails(id: string) {
      const service = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await service.getStudentById(id);
        if (result?.succeeded && result.data) {
          this.selectedStudent = result.data; // Cập nhật selectedStudent với dữ liệu chi tiết
        }
      } catch (error) {
        console.error('Error fetching student details:', error);
      } finally {
        this.loading = false;
      }
    },
    /** Chọn 1 học viên (dùng khi mở dialog) */
    selectStudent(student: StudentModel | null) {
      this.selectedStudent = student;
    },

    /** Lưu hoặc cập nhật học viên */
    async saveStudent(student: Partial<StudentModel>) {
      const studentService = serviceFactory.studentService;
      await studentService.saveStudent(student);
    },

    /** Xóa một hoặc nhiều học viên */
    async deleteStudents(studentIds: string[]) {
      const studentService = serviceFactory.studentService;
      await studentService.deleteStudents(studentIds);
    },

    /** Khôi phục học viên đã xóa (nếu có chức năng) */
    async restoreStudents(studentIds: string[]) {
      const studentService = serviceFactory.studentService;
      await studentService.restoreStudents(studentIds);
    },

    /** Tìm học viên theo mã (gọi server) và trả về bản ghi đầu tiên nếu có */
    async fetchStudentByCode(studentCode: string) {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.getStudentByCode(studentCode);
        if (result?.succeeded === true && result.data) {
          this.selectedStudent = result.data;
          return result.data;
        }
        return null;
      } catch (error) {
        console.error('Error fetching student by code:', error);
        return null;
      } finally {
        this.loading = false;
      }
    },

    /** Đổi trang (phân trang) */
    async setPage(page: number) {
      this.query.page = page;
    },

    /** Đổi số dòng trên trang */
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
    },

    /** Lọc dữ liệu */
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
    },

    /** Lấy danh sách học viên đã xóa (nếu cần) */
    async fetchDeletedStudents() {
      const studentService = serviceFactory.studentService;
      this.loading = true;
      try {
        const result = await studentService.fetchDeletedStudents();
        if (result?.succeeded === true) {
          this.students = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching deleted students:', error);
      } finally {
        this.loading = false;
      }
    },
  },
});
