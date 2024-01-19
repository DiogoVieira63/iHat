import { defineStore } from 'pinia'
import type { Log } from '@/interfaces'
export const useNotificacoesStore = defineStore('notificacoes', {
    state: () => ({
        notificacoes: [] as Array<Log>,
        namesObras: {} as Record<string, string>,
        first: true
    }),
    actions: {
        addNotification(log: Log) {
            this.notificacoes.push(log)
        },
        removeNotification(index: number) {
            this.notificacoes.splice(index, 1)
        },
        unseenNotifications() {
            return this.notificacoes.filter((notification) => !notification.vista)
        }
    }
})
