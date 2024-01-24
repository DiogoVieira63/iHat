<script setup lang="ts">
import ConfirmationDialog from './ConfirmationDialog.vue';

const props = defineProps({
    estado: {
        type: String,
        required: true
    },
    idCapacete: {
        type: Number,
        required: true
    },
    canEdit: {
        type: Boolean,
        default: false
    }
})

const emit = defineEmits(['changeStatus'])

function changeStatus(ConfirmationDialog: boolean) {
    if (ConfirmationDialog) {
        emit('changeStatus', newStatus(props.estado))
    }
}

function newStatus(Status: string) {
    return Status === 'Não Operacional' ? 'Livre' : 'Não Operacional'
}

function isInUso() {
    return props.estado === 'Em Uso'
}

</script>
<template>
    <ConfirmationDialog
        title="Confirmação"
        :function="changeStatus"
    >
        <template #button="{ open }">
            <v-select
                label="Estado do Capacete"
                :disabled="isInUso() || !canEdit"
                class="mt-5"
                density="compact"
                :items="['Livre', 'Não Operacional']"
                :model-value="props.estado"
                @update:model-value="
                    (value) => {
                        if (value !== props.estado) open()
                    }
                "
            >
            </v-select>
        </template>
        <template v-slot:message>
            Tem a certeza que pretende alterar o Status do
            <strong> Capacete {{ props.idCapacete }}</strong> de
            <span class="font-weight-bold text-red">{{
                props.estado
            }}</span>
            para
            <span class="font-weight-bold text-green">{{ newStatus(props.estado) }}</span
            >?
        </template>
    </ConfirmationDialog>
</template>

<style>

</style>