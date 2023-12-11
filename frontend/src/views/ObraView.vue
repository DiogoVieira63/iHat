<script setup lang="ts">
import Lista from '../components/Lista.vue'
import PageLayout from '../components/PageLayout.vue'
import { ref, onMounted, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import RowObra from '@/components/RowObra.vue'
import Map from '@/components/Map.vue'
import Confirmation from '@/components/Confirmation.vue'
import type { Header } from '@/interfaces'
import FormCapaceteObra from '@/components/FormCapaceteObra.vue'

const route = useRoute()
const list = ref<Array<{ [id: string]: string }>>([])

const headers: Array<Header> = [
    { key: 'id', name: 'ID', params: ['sort'] },
    { key: 'Estado', name: 'Estado', params: ['filter', 'sort'] },
    { name: 'Actions', params: [], key: 'actions' }
]

const title = ref('Nome da Obra ' + route.params.id)
const isEditing = ref(false)
const textField = ref<HTMLInputElement | null>(null)
const estadoObra = ref('Planeada')
const newEstado = ref('')

const toggleEditing = () => {
    isEditing.value = !isEditing.value
    if (isEditing.value) {
        nextTick(() => {
            if (textField.value) {
                textField.value.focus()
            }
        })
    }
}

const saveTitle = () => {
    isEditing.value = false
}

onMounted(() => {
    for (let i = 0; i < 30; i++) {
        // random Estado between "Livre" and "Em uso" and "Não Operacional"
        const randomEstado = Math.floor(Math.random() * 3)
        let estado = ''
        if (randomEstado === 0) {
            estado = 'Livre'
        } else if (randomEstado === 1) {
            estado = 'Em uso'
        } else {
            estado = 'Não Operacional'
        }
        list.value.push({ id: String(i), Estado: estado })
    }
})

function removeCapacete(id: number) {
    list.value = list.value.filter((item) => item.id !== String(id))
}

const changeEstadoCapacete = (row: { [key: string]: string }, value: string) => {
    row['Estado'] = value
}

const newEstadoPossible = (value: string) => {
    newEstado.value = value
}

const changeEstado = (value: boolean) => {
    if (value) {
        estadoObra.value = newEstado.value
    }
    newEstado.value = ''
}
</script>
<template>
    <PageLayout>
        <v-row class="mt-2">
            <v-col
                cols="12"
                lg="6"
            >
                <v-row class="d-flex justify-center"
            >
                    <v-col
                        cols="auto"
                        v-bind:offset-lg="4"
                    >
                        <div
                            class="text-h4 text-lg-h3"
                            v-if="!isEditing"
                        >
                            {{ title }}
                        </div>
                        <v-text-field
                            v-else
                            v-model="title"
                            dense
                            @keydown.enter="saveTitle"
                            ref="textField"
                            style="width: 300px"
                        ></v-text-field>
                    </v-col>
                    <v-col cols="auto">
                        <v-btn
                            density="compact"
                            icon="mdi-pencil"
                            @click="toggleEditing"
                        ></v-btn>
                    </v-col>
                    <Map></Map>
                </v-row>
            </v-col>
            <v-col
                cols="12"
                lg="6"
                class="px-16"
            >
                <v-row>
                    <v-spacer></v-spacer>
                    <v-col
                        cols="12"
                        lg="6"
                        xl="4"
                    >
                        <confirmation
                            title="Confirmação"
                            :function="changeEstado"
                        >
                            <template #button="{ open }">
                                <v-select
                                    rounded="t-xl"
                                    label="Estado da Obra"
                                    :items="[
                                        'Planeada',
                                        'Em Curso',
                                        'Pendente',
                                        'Finalizada',
                                        'Cancelada'
                                    ]"
                                    :model-value="estadoObra"
                                    @update:model-value="
                                        (value) => {
                                            newEstadoPossible(value)
                                            open()
                                        }
                                    "
                                >
                                </v-select>
                            </template>
                            <template v-slot:message>
                                Tem a certeza que pretende mudar o estado da obra de
                                <span class="text-red font-weight-bold">{{ estadoObra }}</span> para
                                <span class="text-green font-weight-bold">{{ newEstado }}</span
                                >?
                            </template>
                        </confirmation>
                    </v-col>
                </v-row>
                <Lista
                    v-if="list.length > 0"
                    :list="list"
                    :headers="headers"
                >
                    <template v-slot:tabs>
                        <p class="text-md-h6 ml-2 text-subtitle-1">Lista de Capacetes</p>
                    </template>
                    <template #row="{ row }">
                        <RowObra
                            :row="row"
                            @removeCapacete="(id) => removeCapacete(id)"
                            @changeEstado="(value) => changeEstadoCapacete(row, value)"
                        />
                    </template>
                    <template v-slot:add>
                        <FormCapaceteObra />
                    </template>
                </Lista>
            </v-col>
        </v-row>
    </PageLayout>
</template>
