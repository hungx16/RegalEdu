import { defineStore } from 'pinia'
import { serviceFactory } from '@/services/ServiceFactory'
import type {
  AddTeacherWorkLogCommand,
  GetAllTeacherSessionsQuery,
  ReassignTeacherSessionsCommand,
  TeacherSessionItemModel,
} from '@/api/TeacherSessionApi'

export const useTeacherSessionStore = defineStore('teacherSessions', {
  state: () => ({
    sessions: [] as TeacherSessionItemModel[],
    loading: false,
  }),
  actions: {
    async fetchSessions(query: GetAllTeacherSessionsQuery) {
      this.loading = true
      try {
        const res = await serviceFactory.teacherSessionService.getAllTeacherSessions(query)
        this.sessions = res.data || []
        return res
      } catch (error) {
        console.error('Error fetching teacher sessions:', error)
        this.sessions = []
      } finally {
        this.loading = false
      }
    },
    async reassignTeacherSessions(command: ReassignTeacherSessionsCommand) {
      return await serviceFactory.teacherSessionService.reassignTeacherSessions(command)
    },
    async addTeacherWorkLog(command: AddTeacherWorkLogCommand) {
      return await serviceFactory.teacherSessionService.addTeacherWorkLog(command)
    },
    reset() {
      this.sessions = []
    },
  },
})
