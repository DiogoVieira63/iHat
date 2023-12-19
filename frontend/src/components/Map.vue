<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import Confirmation from '@/components/Confirmation.vue'
import { useDisplay } from 'vuetify'
import FormMapa from './FormMapa.vue'

const { mdAndDown } = useDisplay()

const page = ref(1)
const edit = ref(false)
const imageUrls: Array<string> = ['/Duplex1.svg', '/Duplex2.svg']
const addMapa = ref(false)

const getCurrentImage = computed(() => {
    const currentIndex = page.value - 1
    const validIndex = Math.min(Math.max(currentIndex, 0), imageUrls.length - 1)
    return imageUrls[validIndex]
})

const saveEdit = (confirmation: boolean) => {
    if (confirmation) console.log('Save')
    else console.log('Cancel')
    edit.value = false
}
</script>
<template>
    <template v-if="imageUrls.length > 0">
        <template v-for="image in imageUrls" :key="image">
            <map-editor
                :active="image === getCurrentImage"
                :edit="edit"
                :svg-src="image"
                options="Edit"
            ></map-editor>
        </template>
        <v-container>
            <v-row>
                <v-spacer />
                <v-col cols="10" md="8">
                    <v-pagination v-model="page" :length="imageUrls.length" :total-visible="5">
                    </v-pagination>
                </v-col>
                <v-col cols="2" md="4" class="mt-3">
                    <v-btn
                        v-if="!edit"
                        prepend-icon="$edit"
                        variant="tonal"
                        color="primary"
                        @click="edit = !edit"
                    >
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
        </v-container>
    </template>
    <v-container v-else>
        <v-sheet width="100%" height="900px" class="d-flex justify-center">
            <div class="d-flex align-center">
                <v-sheet class="d-flex flex-column" width="500px">
                    <p class="text-center text-h6">Não existem mapas para esta obra.</p>
                    <v-row>
                        <v-col>
                            <v-btn
                                block
                                color="primary"
                                @click="addMapa = true"
                                class="text-center mt-5"
                            >
                                Adicionar Mapa
                            </v-btn>
                        </v-col>
                        <v-col v-if="addMapa">
                            <v-btn
                                block
                                color="primary"
                                @click="addMapa = false"
                                class="text-center mt-5"
                            >
                                Cancelar
                            </v-btn>
                        </v-col>
                    </v-row>
                    <FormMapa v-if="addMapa" class="mt-2" />
                </v-sheet>
            </div>
        </v-sheet>
    </v-container>
</template>

<style>
.center {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 70vh;
    /* Optional: Center content vertically within the viewport */
}
</style>
