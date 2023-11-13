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
const emit = defineEmits(['removeCapacete'])

const router = useRouter()
const dialog = ref(false)
const removeDialog = ref(false)

function changePage(id) {
    router.push({ path: `/capacete/${id}` })
}

function changeEstado(confirmation) {
    if (confirmation) {
        props.row.Estado = newEstado(props.row.Estado)
    }
    dialog.value = false
}

function newEstado(estado) {
    return estado === "Livre" ? "Não Operacional" : "Livre"
}


function removeCapacete(confirmation) {
    removeDialog.value = false
    if (confirmation)
        emit('removeCapacete', props.row.id)
}

function possibleChangeEstado(value, open) {
    if (value !== props.row['Estado'])
        open()
}

function isInUso() {
    return props.row.Estado === "Em uso"
}

</script>
<template>
    <td @click="changePage(props.row.id)">{{ props.row['id'] }}</td>
    <td>
        <confirmation title="Confirmação" :function="changeEstado">
            <template #button="{ open }">
                <v-select :disabled="isInUso()" class="mt-5 pa-0" density="compact" :items="['Livre', 'Não Operacional']"
                    :model-value="props.row['Estado']" @update:model-value="(value) => possibleChangeEstado(value, open)">
                </v-select>
            </template>
            <template v-slot:message>
                Tem a certeza que pretende alterar o estado do <strong> Capacete {{ props.row.id }}</strong> de
                <span class="font-weight-bold text-red">{{ props.row['Estado'] }}</span> para
                <span class="font-weight-bold text-green">{{ newEstado(props.row['Estado']) }}</span>?
            </template>
        </confirmation>
    </td>
    <td>
        <confirmation title="Confirmação" :function="removeCapacete">
            <template #button="{ prop }">
                <v-btn v-bind="prop" color="grey" text :disabled="isInUso()">
                    <v-icon>mdi-delete</v-icon>
                </v-btn>
            </template>
            <template v-slot:message>
                Tem a certeza que pretende remover o <strong> Capacete {{ props.row.id }}</strong> da obra?
            </template>
        </confirmation>
    </td>
</template>

