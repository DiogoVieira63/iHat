<script setup lang="ts">
import Lista from '../components/Lista.vue'
import PageLayout from '../components/PageLayout.vue'
import { ref, onMounted, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import RowObra from '@/components/RowObra.vue'
import Map from '@/components/Map.vue'
import Confirmation from '@/components/Confirmation.vue'
import FormCapaceteObra from '@/components/FormCapaceteObra.vue'
import type { Capacete, Obra } from '@/interfaces'
import { CapaceteService, ObraService } from '@/http_requests'
import LogsObra from '@/components/LogsObra.vue'


const route = useRoute()
const capacetes = ref<Array<Capacete>>([])
const list = ref<Array<Capacete>>([])

const title = ref('') 
const isEditing = ref(false)
const textField = ref<HTMLInputElement | null>(null)
const estadoObra = ref('')
const newEstado = ref('')
const id = route.params.id

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
    isEditing.value = false;
    const id: string = route.params.id.toString();
    ObraService.updateNomeObra(id, title.value)
        .then(() => {
            getObra()
        })
        .catch((error) => {
            console.error('Error updating title:', error);
        });
};

const getObra = () => {
    ObraService.getOneObra(route.params.id.toString()).then((answer) => {
        console.log(answer)
        if(answer.name) title.value = answer.name
        if(answer.status) estadoObra.value = answer.status
    })
}

const getCapacetesObra = () => {
    capacetes.value = []
    ObraService.getCapacetesFromObra(route.params.id.toString()).then((answer) => {
        answer.forEach((capacete) => {
            capacetes.value.push(capacete)
        })
    })
    list.value = capacetes.value
}

// const getCapacetesFromObra = (id: string) => {
//   console.log("getCapacetesFromObra")
//   list.value = []
//   ObraService.getCapacetesFromObra(id).then((answer) => {
//     console.log(answer)
//     answer.forEach((capacete) => {
//       list.value.push(capacete)
//     })
//   })
// }

onMounted(() => {
    getObra()
    getCapacetesObra()
})

const headers : Array<Header>= [
    { key: 'nCapacete', name: 'Id', params: ['sort'] },
    { key: 'status', name: 'Estado', params: ['filter', 'sort'] },
    { key: 'Actions', name: 'Actions', params: []}
]

function removeCapacete(id: string) {
    const idObra: string = route.params.id.toString();
    ObraService.deleteCapaceteFromObra(idObra, id)
        .then(() => {
            getCapacetesObra()
        })
        .catch((error) => {
            console.error('Error updating title:', error);
        });
}

// onMounted(() => {
//   const id = route.params.id
//     getCapacetesFromObra(id);  
// })


// function removeCapacete(id: number) {
//     list.value = list.value.filter((item) => item.NCapacete !== id)
// }

const changeEstadoCapacete = (row: { [key: string]: string }, value: string) => {
    row['status'] = value
}

const newEstadoPossible = (value: string) => {
    newEstado.value = value
}

const changeEstado = (value: boolean) => {
    const id: string = route.params.id.toString();

    if (value) {
        estadoObra.value = newEstado.value
        ObraService.changeEstadoObra(id, estadoObra.value)
        .then(() => {
            getObra()
        })
        .catch((error) => {
            console.error('Error changing state:', error);
        });
    }
    newEstado.value = ''
}
</script>
<template>
    <PageLayout>
        <v-row class="mt-2">
            <v-col cols="12" lg="6" class="px-16">
                <v-row align="center" justify="start">
                    <v-col cols="auto" v-bind:offset-lg="4">
                        <div class="text-h4 text-lg-h3" v-if="!isEditing">
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
                            @removeCapacete="(nCapacete) => removeCapacete(nCapacete)"
                            @changeStatus="(value) => changeEstadoCapacete(row, value)"
                        />
                    </template>
                    <template v-slot:add>
                        <FormCapaceteObra />
                    </template>
                </Lista>
            </v-col>
        </v-row>
        <LogsObra></LogsObra>

    </PageLayout>
</template>
