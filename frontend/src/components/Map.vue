<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import Confirmation from '@/components/Confirmation.vue'
import { useDisplay } from 'vuetify'
import FormMapa from './FormMapa.vue'
import type { Mapa, Capacete } from '@/interfaces'
import { useRoute } from 'vue-router'
import  type { PropType } from 'vue'

const route = useRoute()
const { mdAndDown } = useDisplay()

const page = ref(1)
const edit = ref(false)
const id : string = route.params.id as string


const props = defineProps({
    mapList: {
        type: Array as PropType<Array<Mapa>>,
        required: true,
    },
    capacetesPosition: {
        type: Array as PropType<Array<Capacete>>,
        required: true,
    }
})

const saveEdit = async (confirmation: boolean) => {
    if (confirmation) {
        console.log('Save')
    } else {
        console.log('Cancel')
    }
    edit.value = false
}


const addMapa = ref(false)
const emit = defineEmits(['update'])

const capacetesMap = computed(() => {
    return props.capacetesPosition.filter((capacete) => {
        if(capacete.position){
            return capacete.position.z == page.value
        }
    })
})


</script>
<template>
    <template v-if="props.mapList.length > 0">
        <template v-for="(map, index) in props.mapList" :key="map.name">
            <map-editor 
                :name="map.name"
                :active="index == page - 1" 
                :edit="edit" 
                :svg="map.svg" 
                :zones="map.zonas"
                :capacetes-position="capacetesMap"
                :haveToolbar = "true"
                @update:zones="map.zonas = $event"
            ></map-editor>
        </template> 
        <v-row class="my-4">
            <v-spacer />
            <v-col cols="10" md="8">
                <v-pagination v-if="props.mapList.length > 1" v-model="page" :length="mapList.length" :total-visible="5">
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
                <v-sheet
                    class="d-flex flex-column" 
                    width="500px"
                >
                    <div
                        v-if="!addMapa" 
                    >
                        <p class="text-center text-h6">Não existem mapas para esta obra.</p>
                        <v-btn  block color="primary" @click="addMapa = true" class="text-center mt-5">
                            Adicionar Mapa
                        </v-btn>
                    </div>
                    <FormMapa 
                        v-else
                        @update="emit('update')"
                        class="mt-2"  :idObra="id" 
                    />
                </v-sheet>
            </div>
        </v-sheet>
    </v-container>
</template>

<style></style>
