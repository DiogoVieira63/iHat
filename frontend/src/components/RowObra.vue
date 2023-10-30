<script setup>
import { useRoute, useRouter } from 'vue-router'
import { ref } from "vue"


const props = defineProps({
    row: {
        type: Object,
        required: true
    },
})

const router = useRouter()
const dialog = ref(false)
const estado = ref(currentEstado(props.row.Estado))
const removeDialog = ref(false)

function changePage(id) {
    router.push({ path: `/capacete/${id}` })
}

function newEstado(value) {
    return value === "Livre" ? "Não Operacional" : "Livre"

}

function currentEstado(value) {
    return value !== "Não Operacional" ? true : false
}

function changeEstado(confirmation) {
    if(confirmation){
        props.row.Estado = newEstado(props.row.Estado)
    }
    else{
        estado.value = !estado.value
    }
    dialog.value = false
}

const emit = defineEmits(['removeCapacete'])

function removeCapacete() {
    removeDialog.value = false
    emit('removeCapacete', props.row.id)
}




</script>
<template>
    <td @click="changePage(props.row.id)">{{ props.row['id'] }}</td>
    <td>{{ props.row['Estado'] }}</td>
    <td>
        <v-row>
            <v-col cols="5">
                <v-dialog v-model="dialog" persistent width="auto">
                    <template v-slot:activator="{ props }">
                        <v-switch v-bind="props" v-model="estado" :disabled="row.Estado == 'Em uso'" color="info"></v-switch>
                    </template>
                    <v-card>
                        <v-card-title class="text-h5">
                            Confirmação
                        </v-card-title>
                        <v-card-text>Tem a certeza que pretende mudar o Estado deste
                            capacete para {{ newEstado(props.row['Estado']) }}?</v-card-text>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="green-darken-1" variant="text" @click="changeEstado(false)">
                                Não
                            </v-btn>
                            <v-btn color="green-darken-1" variant="text" @click="changeEstado(true)">
                                Sim
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-col>
            <v-col cols="6">
                <v-dialog v-model="removeDialog" persistent width="auto">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" color="grey" text class="mt-2">
                            <v-icon>mdi-delete</v-icon>
                        </v-btn>
                    </template>
                    <v-card>
                        <v-card-title class="text-h5">
                            Confirmação
                        </v-card-title>
                        <v-card-text>Tem a certeza que pretende remover o Capacete {{ row.id }} da obra?</v-card-text>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="red" variant="text" @click="removeCapacete">
                                Não
                            </v-btn>
                            <v-btn color="green" variant="text" @click="removeCapacete">
                                Sim
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-col>
        </v-row>
    </td>
</template>

