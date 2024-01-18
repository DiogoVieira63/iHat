<script setup lang="ts">
import FormObra from '@/components/FormObra.vue'
import FormCapacete from '@/components/FormCapacete.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import type { ComputedRef } from 'vue'
import type { Capacete, Obra } from '@/interfaces'
import { CapaceteService, ObraService } from '@/services/http'
import type { Header } from '@/interfaces'
import DataTable from '@/components/DataTable.vue'

const obras = ref<Array<Obra>>([])
const capacetes = ref<Array<Capacete>>([])
const tab = ref('obras')
const router = useRouter()
const isLoaded = ref(false)

const getCapacetes = () => {
    capacetes.value = []
    return CapaceteService.getCapacetes().then((answer) => {
        answer.forEach((capacete) => {
            capacetes.value.push(capacete)
        })
        capacetes.value = capacetes.value.sort((a, b) => {
            return a.nCapacete - b.nCapacete
        })
    })
}

const getObras = () => {
    obras.value = []
    return ObraService.getObras().then((answer) => {
        answer.forEach((obra) => {
            obras.value.push(obra)
        })
        obras.value = obras.value.sort(function (a, b) {
            if (a.name < b.name) return -1
            else if (a.name > b.name) return 1
            else return 0
        })
    })
}

onMounted(async () => {
    await Promise.all([getCapacetes(), getObras()])
    isLoaded.value = true
})

const headers: ComputedRef<Array<Header>> = computed(() => {
    let value: Array<Header> = []
    if (tab.value === 'obras') {
        value = [
            { key: 'name', name: 'Nome', params: ['sort'] },
            { key: 'status', name: 'Estado', params: ['filter', 'sort'] }
        ]
    } else if (tab.value === 'capacetes') {
        value = [
            { key: 'nCapacete', name: 'Id', params: ['sort'] },
            { key: 'status', name: 'Estado', params: ['filter', 'sort'] }
        ]
    }
    return value
})

function changePage(id: string) {
    router.push({ path: `/${tab.value}/${id}` })
}
</script>

<template>
    <PageLayout>
        <v-container>
            <v-skeleton-loader
                :loading="!isLoaded"
                type="card, table"
            >
                <v-card
                    class="mx-auto"
                    max-width="1500px"
                    variant="text"
                >
                    <DataTable
                        :list="tab == 'capacetes' ? capacetes : obras"
                        :headers="headers"
                    >
                        <template v-slot:tabs>
                            <v-tabs
                                v-model="tab"
                                class="rounded-t-xl align-start"
                                bg-color="grey lighten-3"
                                color="black"
                                align-tabs="center"
                            >
                                <v-tab
                                    value="obras"
                                    color="black"
                                    >Obras</v-tab
                                >
                                <v-tab
                                    value="capacetes"
                                    color="black"
                                    >capacetes</v-tab
                                >
                            </v-tabs>
                        </template>
                        <template v-slot:add>
                            <FormObra
                                v-if="tab == 'obras'"
                                @update="getObras"
                            />
                            <FormCapacete
                                v-else
                                @update="getCapacetes"
                            />
                        </template>
                        <template #row="{ row, headers }">
                            <td
                                v-for="{ key } in headers"
                                :key="key"
                                @click="changePage(row['nCapacete'] ? row['nCapacete'] : row['id'])"
                            >
                                {{ row[key] }}
                            </td>
                        </template>
                    </DataTable>
                </v-card>
            </v-skeleton-loader>
        </v-container>
    </PageLayout>
</template>
