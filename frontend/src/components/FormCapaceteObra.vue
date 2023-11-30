<script setup lang="ts">
import { ref } from 'vue'

const id = ref([])
const dialogCapacete = ref(false)

const items = ['Capacete 1', 'Capacete 2', 'Capacete 3']

const submit = () => {
    alert(JSON.stringify(id.value, null, 2))
    id.value = []
    dialogCapacete.value = false
}

const close = () => {
    id.value = []
    dialogCapacete.value = false
}
</script>

<template>
    <v-row justify="center">
        <v-dialog v-model="dialogCapacete" persistent width="600">
            <template v-slot:activator="{ props }">
                <v-btn
                    v-bind="props"
                    color="primary"
                    variant="flat"
                    icon="mdi-plus"
                    rounded="xl"
                    class="ma-1"
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
                            <v-btn icon @click="close" variant="text">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-row>
                    </v-card-title>
                    <v-card-text>
                        <v-autocomplete
                            v-model="id"
                            :items="items"
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
                        ></v-autocomplete>
                        <v-btn type="submit" block class="mt-2" color="primary">Submit</v-btn>
                    </v-card-text>
                </v-form>
            </v-card>
        </v-dialog>
    </v-row>
</template>
