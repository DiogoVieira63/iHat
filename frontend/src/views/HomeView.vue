<script setup lang="ts">
import FormObra from '@/components/FormObra.vue'
import FormCapacete from '@/components/FormCapacete.vue'
import Lista from '@/components/Lista.vue'
import PageLayout from '@/components/PageLayout.vue'
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import type { ComputedRef } from 'vue'

const obras = ref<Array<{ [id: string]: string }>>([])
const capacetes = ref<Array<{ [id: string]: string }>>([])
const list = ref<Array<{ [id: string]: string }>>([])
const tab = ref('obras')
const formObra = ref(true)
const router = useRouter()

onMounted(() => {
    for (let i = 0; i < 30; i++) {
        const randomEstado = Math.floor(Math.random() * 3)
        let estado = ''
        if (randomEstado === 0) {
            estado = 'Livre'
        } else if (randomEstado === 1) {
            estado = 'Em uso'
        } else {
            estado = 'Não Operacional'
        }
        let randomObra = Math.floor(Math.random() * 5)
        let obra = randomObra === 0 ? '' : 'Obra' + randomObra
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
        obras.value.push({ id: String(i), Nome: 'Obra' + i, Estado: estadoObra })
        capacetes.value.push({ id: String(i), Estado: estado, Obra: obra })
    }
    list.value = obras.value
})

const headers: ComputedRef<{ [id: string]: string[] }> = computed(() => {
    if (tab.value === 'obras') {
        return { Nome: ['sort'], Estado: ['filter', 'sort'] }
    } else if (tab.value === 'capacetes') {
        return { id: ['sort'], Estado: ['filter', 'sort'], Obra: ['filter', 'sort'] }
    }
    return {} as { [id: string]: string[] }
})

const changeTab = (newValue: string | null | unknown) => {
    if (newValue === 'obras') {
        list.value = obras.value
        formObra.value = true
        console.log('obras', obras.value, list.value)
    } else if (newValue === 'capacetes') {
        list.value = capacetes.value
        formObra.value = false
        console.log('capacetes', capacetes.value)
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
                <Lista v-if="list.length > 0" :list="list" :headers="headers">
                    <template v-slot:tabs>
                        <v-tabs
                            v-model="tab"
                            class="rounded-t-xl align-start"
                            bg-color="grey lighten-3"
                            color="black"
                            align-tabs="center"
                            @update:model-value="changeTab"
                        >
                            <v-tab value="obras" color="black">Obras</v-tab>
                            <v-tab value="capacetes" color="black">capacetes</v-tab>
                        </v-tabs>
                    </template>
                    <template v-slot:add>
                        <FormObra v-if="formObra" />
                        <FormCapacete v-else />
                    </template>
                    <template #row="{ row, headers }">
                        <td v-for="(_, key) in headers" :key="key" @click="changePage(row['id'])">
                            {{ row[key] }}
                        </td>
                    </template>
                </Lista>
                <v-alert v-else dense type="info">No results found</v-alert>
            </v-sheet>
        </v-container>
    </PageLayout>
</template>
