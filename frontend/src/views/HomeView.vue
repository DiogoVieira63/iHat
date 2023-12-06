<script setup lang="ts">
import FormObra from '@/components/FormObra.vue'
import FormCapacete from '@/components/FormCapacete.vue'
import Lista from '@/components/Lista.vue'
import PageLayout from '@/components/PageLayout.vue'
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import type { ComputedRef } from 'vue'
import type { Capacete, Obra } from '@/interfaces'
import { CapaceteService, ObraService } from '@/http_requests'
import type { Header } from '@/interfaces'

const obras = ref<Array<Obra>>([])
const capacetes = ref<Array<Capacete>>([])
const list = ref<Array<Capacete | Obra>>([])
const tab = ref('capacetes')
const router = useRouter()

onMounted(() => {
    CapaceteService.getCapacetes().then((answer) => {
        answer.forEach((capacete) => {
            capacetes.value.push(capacete)
        })
    })
    for (let i = 0; i < 30; i++) {
        let randomObra = Math.floor(Math.random() * 5)
        let randomEstadoObra = Math.floor(Math.random() * 5)
        let estadoObra = ''
        if (randomEstadoObra === 0) {
            estadoObra = 'Pendente'
        } else if (randomEstadoObra === 1) {
            estadoObra = 'Em Curso'
        } else if (randomEstadoObra === 2) {
            estadoObra = 'Concluída'
        } else if (randomEstadoObra === 3) {
            estadoObra = 'Cancelada'
        } else {
            estadoObra = 'Planeada'
        }
        let obra: Obra = {
            _id: String(i),
            name: 'Obra' + i,
            idResponsavel: 0,
            status: estadoObra,
        }
        obras.value.push(obra)
    }
    list.value = capacetes.value
})

const headers: ComputedRef<Array<Header>> = computed(() => {
    let value: Array<Header> = []
    if (tab.value === 'obras') {
        value = [
            { key: 'name', name: 'Nome', params: ['sort'] },
            { key: 'status', name: 'Estado', params: ['filter', 'sort'] },

        ]
    } else if (tab.value === 'capacetes') {
        value = [
            { key: 'nCapacete', name: 'Id', params: ['sort'] },
            { key: 'status', name: 'Estado', params: ['filter', 'sort'] },
        ]
    }
    return value
})

const changeTab = (newValue: string | null | unknown) => {
    if (newValue === 'obras') {
        list.value = obras.value
    } else if (newValue === 'capacetes') {
        list.value = capacetes.value
    }
}

function changePage(id: string) {
    router.push({ path: `/${tab.value}/${id}` })
}
</script>

<template>
    <PageLayout>
        <v-container>
            <v-sheet class="mx-auto" max-width="1500px">
                <Lista :list="list" :headers="headers">
                    <template v-slot:tabs>
                        <v-tabs v-model="tab" class="rounded-t-xl align-start" bg-color="grey lighten-3" color="black"
                            align-tabs="center" @update:model-value="changeTab">
                            <v-tab value="obras" color="black">Obras</v-tab>
                            <v-tab value="capacetes" color="black">capacetes</v-tab>
                        </v-tabs>
                    </template>
                    <template v-slot:add>
                        <FormObra v-if="tab == 'obras'" />
                        <FormCapacete v-else />
                    </template>
                    <template #row="{ row, headers }">
                        <td v-for="{ key } in headers" :key="key" @click="changePage(row['id'])">
                            {{ row[key] }}
                        </td>
                    </template>
                </Lista>
            </v-sheet>
        </v-container>
    </PageLayout>
</template>
