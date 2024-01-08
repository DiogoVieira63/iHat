<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Capacete } from '@/interfaces';
import {  CapaceteService, ObraService } from '@/services/http'

const props = defineProps({
    idObra: {
        type: String,
        required: true
    }
})

const id = ref([])
const dialogCapacete = ref(false)
const capacetesLivres = ref<Array<Capacete>>([])

const getCapacetesLivres = () => {
    capacetesLivres.value = []
    CapaceteService.getCapacetesLivres().then((answer) => {
        answer.forEach((capacete) => {
            capacetesLivres.value.push(capacete)
        })
    })
}

// onMounted( () => {
//     getCapacetesLivres()
// });

const emit = defineEmits(['update'])

const submit = () => {
    id.value.forEach(idCapacete => {
        ObraService.addCapaceteToObra(props.idObra, idCapacete)
            .then(() => {
                emit('update')
            })
            .catch((error) => {
                console.log(error)
            })
        })
    // alert(JSON.stringify(id.value, null, 2))
    id.value = []
    dialogCapacete.value = false
}

const close = () => {
    id.value = []
    dialogCapacete.value = false
}

const capacetesLivresWithTitle = () => {
    return capacetesLivres.value.map(item => ({
        ...item,
        title: `Capacete ${item.nCapacete}`
    }));
}
</script>

<template>
    <v-row justify="center">
        <v-dialog
            v-model="dialogCapacete"
            persistent
            width="600"
        >
            <template v-slot:activator="{ props }">
                <v-btn
                    v-bind="props"
                    color="primary"
                    variant="flat"
                    icon="mdi-plus"
                    rounded="xl"
                    class="ma-1"
                    @click="getCapacetesLivres"
                >
                </v-btn>
            </template>
            <v-card>
                <v-form @submit.prevent="submit">
                    <v-card-title class="text-center">
                        <v-row class="mt-1">
                            <v-spacer></v-spacer>
                            <h1 class="text-h5">Adicionar Capacete à Obra</h1>
                            <v-spacer></v-spacer>
                            <v-btn
                                icon
                                @click="close"
                                variant="text"
                            >
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-row>
                    </v-card-title>
                    <v-card-text>
                        <v-autocomplete
                            v-model="id"
                            :items="capacetesLivresWithTitle()"
                            auto-select-first
                            density="comfortable"
                            menu-icon=""
                            placeholder="Procure o Capacete"
                            prepend-inner-icon="mdi-magnify"
                            hide-selected
                            no-data-text="Nenhum Capacete encontrado"
                            rounded
                            multiple
                            closable-chips
                            chips
                            transition="false"
                            variant="solo"
                            item-title="title"
                            item-value="nCapacete"
                        ></v-autocomplete>
                        <v-btn
                            type="submit"
                            block
                            class="mt-2"
                            color="primary"
                            >Submit</v-btn
                        >
                    </v-card-text>
                </v-form>
            </v-card>
        </v-dialog>
    </v-row>
</template>
