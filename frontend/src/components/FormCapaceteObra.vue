<script setup lang="ts">
import { ref} from 'vue'
import type { Capacete } from '@/interfaces'
import { CapaceteService, ObraService } from '@/services/http'

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
        capacetesLivresWithTitle()
    })
}

const emit = defineEmits(['update'])

const submit = async () => {
    try {
        for (const idCapacete of id.value) {
            await ObraService.addCapaceteToObra(props.idObra, idCapacete)
        }
        emit('update')
        id.value = []
        dialogCapacete.value = false
    } catch (error) {
        console.error(error)
    }
}

const close = () => {
    id.value = []
    dialogCapacete.value = false
}

const capacetesLivresWithTitle = () => {
    capacetesLivres.value = capacetesLivres.value.map((item) => ({
        ...item,
        title: `Capacete ${item.numero}`
    }))
    capacetesLivres.value = capacetesLivres.value.sort((a, b) => {
        return a.numero - b.numero
    })
    return capacetesLivres.value
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
                            :items="capacetesLivres"
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
                            item-value="numero"
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
