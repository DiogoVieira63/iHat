<script setup lang="ts">
import { useRouter } from 'vue-router'
import { ref } from 'vue'
import ConfirmationDialog from './ConfirmationDialog.vue'

interface Props {
    row: { [nCapacete: string]: string }
    selected: boolean
}

const props = defineProps<Props>()

const emit = defineEmits(['removeCapacete', 'changeStatus', 'selectCapacete'])

const router = useRouter()
const dialog = ref(false)
const removeDialog = ref(false)

function changePage(nCapacete: string) {
    router.push({ path: `/capacetes/${nCapacete}` })
}

function changeStatus(ConfirmationDialog: boolean) {
    if (ConfirmationDialog) {
        emit('changeStatus', newStatus(props.row.status))
    }
    dialog.value = false
}

function newStatus(Status: string) {
    return Status === 'Não Operacional' ? 'Livre' : 'Não Operacional'
}

function removeCapacete(ConfirmationDialog: boolean) {
    removeDialog.value = false
    if (ConfirmationDialog) emit('removeCapacete', props.row.nCapacete)
}

function isInUso() {
    return props.row.status === 'Em Uso'
}
</script>
<template>
    <td
        style="cursor: pointer"
        @click="emit('selectCapacete', props.row.nCapacete)"
    >
        {{ props.row['nCapacete'] }}
    </td>
    <td>
        <ConfirmationDialog
            title="Confirmação"
            :function="changeStatus"
        >
            <template #button="{ open }">
                <v-select
                    :disabled="isInUso()"
                    class="mt-5 pa-0"
                    density="compact"
                    :items="['Livre', 'Não Operacional']"
                    :model-value="
                        props.row['status'] == 'Associado à Obra' ? 'Livre' : props.row['status']
                    "
                    @update:model-value="
                        (value) => {
                            if (value !== props.row['status']) open()
                        }
                    "
                >
                </v-select>
            </template>
            <template v-slot:message>
                Tem a certeza que pretende alterar o Status do
                <strong> Capacete {{ props.row.nCapacete }}</strong> de
                <span class="font-weight-bold text-red">{{
                    props.row['status'] == 'Associado à Obra' ? 'Livre' : props.row['status']
                }}</span>
                para
                <span class="font-weight-bold text-green">{{ newStatus(props.row['status']) }}</span
                >?
            </template>
        </ConfirmationDialog>
    </td>
    <td>
        <v-btn
            color="grey"
            variant="text"
            @click="changePage(props.row.nCapacete)"
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
                    :disabled="isInUso()"
                >
                    <v-icon>mdi-delete</v-icon>
                </v-btn>
            </template>
            <template v-slot:message>
                Tem a certeza que pretende remover o
                <strong> Capacete {{ props.row.nCapacete }}</strong> da obra?
            </template>
        </ConfirmationDialog>
    </td>
</template>
