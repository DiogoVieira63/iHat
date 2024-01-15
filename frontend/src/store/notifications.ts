// src/store/index.ts
import { defineStore } from 'pinia'

export class Notification {
    title: string
    time: Date
    seen: boolean

    constructor(title: string) {
        this.title = title
        this.time = new Date()
        this.seen = false
    }
}

export const notificacoes = defineStore('notificacoes', {
    state: () => ({
        notificacoes: [] as Array<Notification>,
    }),
    actions: {
        addNotification(title: string) {
            this.notificacoes.push(new Notification(title))
        },
        removeNotification(index: number) {
            this.notificacoes.splice(index, 1)
        }
    }
})