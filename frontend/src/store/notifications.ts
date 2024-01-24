import type { Log } from '@/interfaces'
import { ObraSignalRService } from '@/services/obraSignalR'
import { defineStore } from 'pinia'
export const useNotificacoesStore = defineStore('notificacoes', {
    state: () => ({
        notificacoes: [] as Array<Log>,
        namesObras: {} as Record<string, string>,
        connections: {} as Record<string,ObraSignalRService>,
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
        },
        startConnection(obraId: string, obraName: string) {
            if (this.connections[obraId]) return
            const connection = new ObraSignalRService(obraId)
            connection.start()
            this.connections[obraId] = connection
            this.namesObras[obraId] = obraName
            connection.handleIncomingLogs(this.addNotification)
        },
        stopConnection(obraId: string) {
            this.connections[obraId].close()
            delete this.connections[obraId]
            delete this.namesObras[obraId]
        },
    }
})
