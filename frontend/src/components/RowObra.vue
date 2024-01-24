<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import ConfirmationDialog from './ConfirmationDialog.vue';
import CapaceteStatus from './CapaceteStatus.vue';
interface Props {
    row: { [numero: string]: string }
    selected: boolean
    canEdit: boolean
}

const props = defineProps<Props>()

const emit = defineEmits(['removeCapacete', 'changeStatus', 'selectCapacete'])

const router = useRouter()
const removeDialog = ref(false)

function changePage(numero: string) {
    router.push({ path: `/capacetes/${numero}` })
}

function changeStatus(ConfirmationDialog: boolean) {
    if (ConfirmationDialog) {
        emit('changeStatus', newStatus(props.row.status))
    }
}

function newStatus(Status: string) {
    return Status === 'Não Operacional' ? 'Livre' : 'Não Operacional'
}

function removeCapacete(ConfirmationDialog: boolean) {
    removeDialog.value = false
    if (ConfirmationDialog) emit('removeCapacete', props.row.numero)
}

function isInUso() {
    return props.row.status === 'Em Uso'
}
</script>
<template>
    <td
        style="cursor: pointer"
        @click="emit('selectCapacete', props.row.numero)"
    >
        {{ props.row['numero'] }}
    </td>
    <td>
        <CapaceteStatus 
            :estado="props.row['status']"
            :idCapacete="Number(props.row['numero'])"
            :canEdit="props.canEdit"
            @changeStatus="changeStatus"
        >
        </CapaceteStatus>
    </td>
    <td>
        <v-btn
            color="grey"
            variant="text"
            @click="changePage(props.row.numero)"
        >
            <v-icon>mdi-eye</v-icon>
        </v-btn>
        <ConfirmationDialog
            title="Confirmação"
            :function="removeCapacete"
        >
            <template #button="{ prop }">
                <v-btn
                    v-bind="prop"
                    color="grey"
                    variant="text"
                    :disabled="isInUso() || !canEdit"
                >
                    <v-icon>mdi-delete</v-icon>
                </v-btn>
            </template>
            <template v-slot:message>
                Tem a certeza que pretende remover o
                <strong> Capacete {{ props.row.numero }}</strong> da obra?
            </template>
        </ConfirmationDialog>
    </td>
</template>
