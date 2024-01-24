<script setup lang="ts">
import BaseLogs from '@/components/BaseLogs.vue'
import ConfirmationDialog from '@/components/ConfirmationDialog.vue'
import DataTable from '@/components/DataTable.vue'
import FormCapaceteObra from '@/components/FormCapaceteObra.vue'
import ObraLayout from '@/components/Layouts/ObraLayout.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import LogsObra from '@/components/LogsObra.vue'
import RowObra from '@/components/RowObra.vue'
import TheMap from '@/components/TheMap.vue'
import type { Capacete, Header, Mapa, Position } from '@/interfaces'
import { CapaceteService, ObraService } from '@/services/http'
import { useNotificacoesStore } from '@/store/notifications'
import { computed, nextTick, onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()
const title = ref('')
const capacetes = ref<Array<Capacete>>([])
const capacetesSelected = ref<number>()
const list = ref<Array<Capacete>>([])
const isEditing = ref(false)
const textField = ref<HTMLInputElement | null>(null)
const estadoObra = ref('')
const newEstado = ref('')
const mapList = ref<Array<Mapa>>([])
const page = ref(route.query.page ? parseInt(route.query.page as string) : 1)
const idObra: string = route.params.id.toString()
const isLoaded = ref(false)
const notificacoesStore = useNotificacoesStore()

const logs = computed(() => {
    return notificacoesStore.notificacoes
        .filter((item) => {
            return item.idObra == idObra
        })
        .slice()
        .reverse()
})

const canEdit = computed(() => {
    return estadoObra.value != 'Finalizada' && estadoObra.value != 'Cancelada'
})

const canSimulador = computed(() => {
    return estadoObra.value == 'Em Curso'
})

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
    const id: string = idObra
    ObraService.updateNomeObra(id, title.value)
        .then(() => {
            getObra()
        })
        .catch((error) => {
            console.error('Error updating title:', error)
        })
}

const getObra = () => {
    return ObraService.getOneObra(idObra).then((answer) => {
        if (answer.mapa) mapList.value = answer.mapa
        if (answer.nome) title.value = answer.nome
        if (answer.status) estadoObra.value = answer.status
        if (estadoObra.value === 'Em Curso') {
            if (!(idObra in notificacoesStore.connections)){
                notificacoesStore.startConnection(idObra, title.value)
            }
            const connection  = notificacoesStore.connections[idObra]
            connection.updateCapacetePosition(updateCapacetePosition)
        }
        else{
            if (idObra in notificacoesStore.connections){
                notificacoesStore.stopConnection(idObra)
            }
        }
    })
}

const getCapacetesObra = () => {
    capacetes.value = []
    return ObraService.getCapacetesFromObra(idObra).then((answer) => {
        capacetes.value = []
        answer.forEach((capacete) => {
            capacetes.value.push(capacete)
        })
        capacetes.value = capacetes.value.sort((a, b) => {
            return a.numero - b.numero
        })
        list.value = capacetes.value
    })
}


const getLastLocationObra = () => {
    return ObraService.getLocationCapacetes(idObra).then((answer) => {
        Object.keys(answer).forEach((key) => {
            const capacete = capacetes.value.find((capacete) => capacete.numero === parseInt(key))
            if (capacete) {
                capacete.position = answer[key]
            }
        })
    })
}

const updateCapacetePosition = (id: number, pos: Position) => {
    capacetes.value = capacetes.value.map((item) => {
        if (item.numero === id) {
            item.position = {
                x: pos.x,
                y: pos.y,
                z: pos.z
            }
            return item
        }
        return item
    })
}

onMounted(async () => {
    await Promise.all([
        getObra(),
        getCapacetesObra(),
        getLastLocationObra(),
    ])
    isLoaded.value = true
})

onUnmounted(() => {
    if (idObra in notificacoesStore.connections){
        const connection  = notificacoesStore.connections[idObra]
        connection.offCapacetePosition()
    }
})

const headers: Array<Header> = [
    { key: 'numero', name: 'Id', params: ['sort','search'] },
    { key: 'status', name: 'Estado', params: ['filter', 'sort','search'] },
    { key: 'Actions', name: 'Actions', params: [] }
]

function removeCapacete(id: string) {
    ObraService.deleteCapaceteFromObra(idObra, id)
        .then(() => {
            getCapacetesObra()
        })
        .catch((error) => {
            console.error('Error updating title:', error)
        })
}

const changeEstadoCapacete = async (idCapacete: number, value: string) => {
    await CapaceteService.changeEstadoCapacete(idCapacete, value)
        .then(() => {
            getCapacetesObra()
        })
        .catch((error) => {
            console.error('Error changing state:', error)
        })
}

const newEstadoPossible = (value: string) => {
    newEstado.value = value
}

const changeEstado = (value: boolean) => {
    if (value) {
        estadoObra.value = newEstado.value
        ObraService.changeEstadoObra(idObra, estadoObra.value)
            .then(() => {
                getObra()
                isEditing.value = false
            })
            .catch((error) => {
                console.error('Error changing state:', error)
            })
    }
    newEstado.value = ''
}

const goToSimulador = () => {
    const currentRoute = router.currentRoute.value
    router.push({path: currentRoute.path + '/simulador', query: {page: page.value}})
}

const selectCapacete = (idCapacete: number) => {
    if (capacetesSelected.value === idCapacete) {
        capacetesSelected.value = undefined
    } else {
        capacetesSelected.value = idCapacete
    }
}
</script>
<template>
    <PageLayout>
        <ObraLayout>
            <template #title>
                <v-row
                    class="my-3 d-flex justify-center"
                >
                <v-col
                    cols="12"
                    md="6"
                    class="d-flex justify-center"
                >
                    <div
                        class="text-h4 text-lg-h3"
                        v-if="!isEditing"
                    >
                        {{ title }} 
                    </div>
                    <v-responsive
                        v-else
                        max-width="344"
                    >
                        <v-text-field
                            v-model="title"
                            @keydown.enter="saveTitle"
                            ref="textField"
                        ></v-text-field>
                    </v-responsive>
                </v-col>
                <v-col cols="12" md="auto" class="mx-4">
                    <ConfirmationDialog
                        title="Confirmação"
                        :function="changeEstado"
                        v-if="isEditing"
                    >
                        <template #button="{ open }">
                            <v-select
                                rounded="t-xl"
                                label="Estado da Obra"
                                :items="['Em Curso', 'Pendente', 'Finalizada', 'Cancelada']"
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
                    </ConfirmationDialog>
                    <v-alert
                        v-else
                        dense
                        class="mx-4 rounded-pill"
                        color="info"
                    >
                        Estado da Obra: {{ estadoObra }}
                    </v-alert>
                </v-col>
                <v-spacer />
                <v-col class="d-flex justify-end mr-6" cols="auto">
                    <v-tooltip text="A obra necessita de estar Em Curso" location="bottom" :disabled="canSimulador">
                        <template #activator="{ props}">
                            <div v-bind="props" >
                                <v-btn
                                rounded="xl"
                                size="large"
                                variant="flat"
                                color="primary"
                                block
                                @click="goToSimulador"
                                :disabled="!canSimulador"
                                >
                                Simulador
                            </v-btn>
                        </div>
                    </template>
                    </v-tooltip>
                    <v-btn
                        rounded="xl"
                        :variant="isEditing ? 'outlined' : 'flat'"
                        color="primary"
                        icon="mdi-pencil"
                        @click="toggleEditing"
                        class="ml-4"
                        :disabled="!canEdit"
                    ></v-btn>
                </v-col>
                </v-row>
            </template>
            <template #map>
                <v-skeleton-loader
                    v-if="!isLoaded"
                    type="card, image"
                    height="65vh"
                >
                </v-skeleton-loader>
                <TheMap
                    :capacetesPosition="capacetes"
                    :capacetesSelected="capacetesSelected"
                    :mapList="mapList"
                    :canEdit="canEdit"
                    :page="page"
                    @updatePage="page = $event"
                    @update="getObra"
                    @selectCapacete="selectCapacete"
                ></TheMap>
            </template>
            <template #content>
                <v-skeleton-loader
                    v-if="!isLoaded"
                    type="card, table"
                >
                </v-skeleton-loader>
                <DataTable
                    :list="list"
                    :headers="headers"
                    :selected="{
                        key: 'numero',
                        value: capacetesSelected
                    }"
                >
                    <template v-slot:tabs>
                        <p class="text-md-h6 ml-2 text-subtitle-1">Lista de Capacetes</p>
                    </template>
                    <template #row="{ row }">
                        <RowObra
                            :selected="capacetesSelected == row['numero']"
                            :row="row"
                            :canEdit="canEdit"
                            @removeCapacete="(numero) => removeCapacete(numero)"
                            @changeStatus="(value) => changeEstadoCapacete(Number(row['numero']), value)"
                            @selectCapacete="selectCapacete"
                        />
                    </template>
                    <template v-slot:add>
                        <FormCapaceteObra
                            v-if="canEdit"
                            :idObra="idObra"
                            @update="getCapacetesObra"
                        />
                    </template>
                </DataTable>
            </template>
            <template #logs>
                <BaseLogs
                    :list="capacetes.map((capacete) => capacete.numero)"
                    :selected="capacetesSelected"
                    @select="capacetesSelected = $event"
                    title="Selecionar Capacete"
                >
                    <LogsObra
                        :logs="logs"
                        :capacete-selected="capacetesSelected"
                        @selectCapacete="selectCapacete"
                    ></LogsObra>
                </BaseLogs>
            </template>
        </ObraLayout>
    </PageLayout>
</template>
