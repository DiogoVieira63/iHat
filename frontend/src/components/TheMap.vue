<script setup lang="ts">
import MapEditor from '@/components/MapEditor.vue'
import type { Capacete, Mapa } from '@/interfaces'
import { ObraService } from '@/services/http'
import type { PropType } from 'vue'
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { useDisplay } from 'vuetify'
import ConfirmationDialog from './ConfirmationDialog.vue'
import FormMapa from './FormMapa.vue'

const route = useRoute()
const { mdAndDown } = useDisplay()

const edit = ref(false)
const id: string = route.params.id as string
const addMapa = ref(false)
const emit = defineEmits(['update', 'selectCapacete', 'updatePage'])

const props = defineProps({
    mapList: {
        type: Array as PropType<Array<Mapa>>,
        required: true
    },
    capacetesPosition: {
        type: Array as PropType<Array<Capacete>>,
        required: true
    },
    capacetesSelected: {
        type: Number,
        default: -1
    },
    canEdit: {
        type: Boolean,
        default: false
    },
    page: {
        type: Number,
        default: 1
    }
})

const saveEdit = async (ConfirmationDialog: boolean) => {
    if (ConfirmationDialog) {
        const zonas = props.mapList.map((map) => {
            return {
                idMapa: map.id,
                zonas: map.zonas
            }
        })
        const promises = []
        for (const zona of zonas) {
            promises.push(ObraService.updateZonasRisco(id, zona.idMapa, zona.zonas))
        }
        await Promise.all(promises)
        emit('update')
    } else {
        console.log('Cancel')
    }
    edit.value = false
}




const capacetesMap = (floor: number) => {
    return props.capacetesPosition.filter((capacete) => {
        if (capacete.position) {
            return capacete.position.z == floor && capacete.status == 'Em Uso'
        }
    })
}
</script>
<template>
    <template v-if="props.mapList.length > 0">
        <template
            v-for="(map, index) in props.mapList"
            :key="map.name"
        >
            <map-editor
                :name="map.name"
                :active="index == page - 1"
                :edit="edit"
                :svg="map.svg"
                :zones="map.zonas"
                :capacetes-position="capacetesMap(map.floor)"
                :capacetesSelected="[props.capacetesSelected]"
                :haveToolbar="true"
                @update:zones="map.zonas = $event"
                @selectCapacete="emit('selectCapacete', $event)"
            ></map-editor>
        </template>
        <v-row class="my-4">
            <v-spacer />
            <v-col
                cols="10"
                md="8"
            >
                <v-pagination
                    v-if="props.mapList.length > 1"
                    :model-value="props.page"
                    @update:model-value="emit('updatePage', $event)"
                    :length="mapList.length"
                    :total-visible="5"
                >
                </v-pagination>
            </v-col>
            <v-col
                cols="2"
                md="4"
                class="mt-3"
            >
                <v-btn
                    v-if="!edit"
                    prepend-icon="$edit"
                    variant="tonal"
                    color="primary"
                    @click="edit = !edit"
                    :disabled="!props.canEdit"
                >
                    {{ mdAndDown ? '' : 'Editar' }}
                </v-btn>
                <ConfirmationDialog
                    v-else
                    title="Confirmação"
                    :function="saveEdit"
                >
                    <template #button="{ prop }">
                        <v-btn
                            v-bind="prop"
                            color="primary"
                            prepend-icon="mdi-content-save"
                        >
                            {{ mdAndDown ? '' : 'Terminar Edição' }}
                        </v-btn>
                    </template>
                    <template v-slot:message> Pretende guardar a edição do mapa? </template>
                </ConfirmationDialog>
            </v-col>
            <v-spacer />
        </v-row>
    </template>
    <v-sheet
        v-else-if="props.canEdit"
        height="75vh"
        class="d-flex justify-center rounded-xl"
    >
        <div class="d-flex align-center">
            <v-sheet
                class="d-flex flex-column"
                width="500px"
            >
                <div v-if="!addMapa">
                    <p class="text-center text-h6">Não existem mapas para esta obra.</p>
                    <v-btn
                        block
                        color="primary"
                        @click="addMapa = true"
                        class="text-center mt-5"
                    >
                        Adicionar Mapa
                    </v-btn>
                </div>
                <FormMapa
                    v-else
                    @update="emit('update')"
                    class="mt-2"
                    :idObra="id"
                />
            </v-sheet>
        </div>
    </v-sheet>
    <v-sheet
        v-else
        height="75vh"
        class="d-flex align-center justify-center rounded-xl"
    >
        <p class="text-center text-h6">Não existem mapas para esta obra.</p>
    </v-sheet>
</template>

<style></style>
