<script setup>
import { useRoute, useRouter } from 'vue-router'
import { ref, computed } from "vue"
import Confirmation from './Confirmation.vue';


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
    if (confirmation) {
        props.row.Estado = newEstado(props.row.Estado)
    }
    else {
        estado.value = !estado.value
    }
    dialog.value = false
}

const emit = defineEmits(['removeCapacete'])

function removeCapacete(confirmation) {
    removeDialog.value = false
    if (confirmation)
        emit('removeCapacete', props.row.id)
}

function isInUso() {
    return props.row.Estado === "Em uso"
}

</script>
<template>
    <td @click="changePage(props.row.id)">{{ props.row['id'] }}</td>
    <td>{{ props.row['Estado'] }}</td>
    <td>
        <v-row>
            <v-col cols="5">
                <confirmation title="Confirmação" :function="changeEstado">
                    <template #button="{ prop }">
                        <v-switch v-bind="prop" v-model="estado" :disabled="row.Estado == 'Em uso'" color="info"></v-switch>
                    </template>
                    <template v-slot:message>
                        Tem a certeza que pretende mudar o Estado do<strong> Capacete {{ props.row.id }}</strong> de
                        <span class='text-red'> {{ props.row.Estado }}</span> para
                        <span class="text-green"> {{ newEstado(props.row['Estado']) }}?</span>
                    </template>
                </confirmation>
            </v-col>
            <confirmation title="Confirmação"  :function="removeCapacete">
                <template #button="{ prop }">
                    <v-btn v-bind="prop" color="grey" text class="mt-6" :disabled="isInUso()">
                        <v-icon>mdi-delete</v-icon>
                    </v-btn>
                </template>
                <template v-slot:message>
                    Tem a certeza que pretende remover o <strong> Capacete {{ props.row.id }}</strong> da obra?
                </template>
            </confirmation>
        </v-row>
    </td>
</template>

