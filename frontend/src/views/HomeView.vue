<script setup lang="ts">
import DataTable from '@/components/DataTable.vue'
import FormCapacete from '@/components/FormCapacete.vue'
import FormObra from '@/components/FormObra.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import type { Capacete, Header, Obra } from '@/interfaces'
import { CapaceteService, ObraService } from '@/services/http'
import type { ComputedRef } from 'vue'
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

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
            return a.numero - b.numero
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
            if (a.nome < b.nome) return -1
            else if (a.nome > b.nome) return 1
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
            { key: 'nome', name: 'Nome', params: ['sort'] },
            { key: 'status', name: 'Estado', params: ['filter', 'sort'] }
        ]
    } else if (tab.value === 'capacetes') {
        value = [
            { key: 'numero', name: 'Id', params: ['sort'] },
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
        <v-skeleton-loader
            v-if="!isLoaded"
            type="card, table"
        >
        </v-skeleton-loader>
        <v-row justify="center" v-else>
            
        <v-col
            cols="12"
            md="10"
            lg="8"
            xl="6"
        >

        <v-card
            class="mx-2 my-10"
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
                        @click="changePage(row['numero'] ? row['numero'] : row['id'])"
                    >
                        {{ row[key] }}
                    </td>
                </template>
            </DataTable>
        </v-card>
    </v-col>
</v-row>
    </PageLayout>
</template>
