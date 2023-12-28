<script setup lang="ts">
import { ref } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import Confirmation from '@/components/Confirmation.vue'
import { useDisplay } from 'vuetify'
import FormMapa from './FormMapa.vue'
import type { Mapa } from '@/interfaces'
import { onMounted } from 'vue'
import mapa from '@/data/map.json'
import { useRoute } from 'vue-router'

const route = useRoute()
const { mdAndDown } = useDisplay()

const page = ref(1)
const edit = ref(false)
const addMapa = ref(false)
const id : string = route.params.id as string



const getMapList = () => {
    //return mapList.value
}


onMounted(() => {
    getMapList()
    mapList.value = mapa
})

const mapList = ref<Array<Mapa>>([])

const saveEdit = async (confirmation: boolean) => {
    if (confirmation) {
        console.log(mapList.value, typeof mapList.value)
        console.log('Save')
    } else {
        console.log('Cancel')
    }
    edit.value = false
}
</script>
<template>
    <template v-if="mapList.length > 0">
        <template v-for="(map, index) in mapList" :key="map.name">
            <map-editor 
                :active="index == page - 1" 
                :edit="edit" 
                :svg="map.svg" 
                :zones="map.zones"
                @update:zones="map.zones = $event"
                options="Edit"
            ></map-editor>
        </template>
        <v-row class="my-4">
            <v-spacer />
            <v-col cols="10" md="8">
                <v-pagination v-if="mapList.length > 1" v-model="page" :length="mapList.length" :total-visible="5">
                </v-pagination>
            </v-col>
            <v-col cols="2" md="4" class="mt-3">
                <v-btn v-if="!edit" prepend-icon="$edit" variant="tonal" color="primary" @click="edit = !edit">
                    {{ mdAndDown ? '' : 'Editar' }}
                </v-btn>
                <confirmation v-else title="Confirmação" :function="saveEdit">
                    <template #button="{ prop }">
                        <v-btn v-bind="prop" color="primary" prepend-icon="mdi-content-save">
                            {{ mdAndDown ? '' : 'Terminar Edição' }}
                        </v-btn>
                    </template>
                    <template v-slot:message> Pretende guardar a edição do mapa? </template>
                </confirmation>
            </v-col>
            <v-spacer />
        </v-row>
    </template>
    <v-container v-else>
        <v-sheet width="100%" height="900px" class="d-flex justify-center">
            <div class="d-flex align-center">
                <v-sheet class="d-flex flex-column" width="500px">
                    <p class="text-center text-h6">Não existem mapas para esta obra.</p>
                    <v-row>
                        <v-col>
                            <v-btn block color="primary" @click="addMapa = true" class="text-center mt-5">
                                Adicionar Mapa
                            </v-btn>
                        </v-col>
                        <v-col v-if="addMapa">
                            <v-btn block color="primary" @click="addMapa = false" class="text-center mt-5">
                                Cancelar
                            </v-btn>
                        </v-col>
                    </v-row>
                    <FormMapa v-if="addMapa" class="mt-2"  :idObra="id" />
                </v-sheet>
            </div>
        </v-sheet>
    </v-container>
</template>

<style></style>
