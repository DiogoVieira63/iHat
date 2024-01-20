<script setup lang="ts">
import { ObraService } from '@/services/http';
import { ref } from 'vue';

const props = defineProps({
    idObra: {
        type: String,
        required: true
    },
    canCancel: {
        type: Boolean,
        default: false
    }
})

const tipo = ref('ifc')

const filesIfc = ref<File[] | undefined>(undefined)
const filesDxf = ref<File[] | undefined>(undefined)
const isUploading = ref(false)

const emit = defineEmits(['update'])

const submit = (values: any) => {
    // post to backend
    let file: File = new File([], '')
    if (tipo.value == 'ifc' && filesIfc.value) {
        file = filesIfc.value[0]
    }

    const promise = ObraService.addMapaToObra(props.idObra, file)
    isUploading.value = true
    promise
        .then(() => {
            emit('update')
            isUploading.value = false
        })
        .catch((error) => {
            console.log(error)
        })
    console.log(JSON.stringify(values, null, 2))
}
</script>

<template>
    <v-form
        @submit.prevent="submit"
        v-if="!isUploading"
    >
        <v-radio-group
            v-model="tipo"
            inline
            class="d-flex justify-center my-5"
        >
            <v-radio
                label="IFC"
                value="ifc"
            />
            <v-radio
                label="DXF"
                value="dxf"
                disabled
            />
        </v-radio-group>
        <v-file-input
            v-model="filesIfc"
            v-if="tipo == 'ifc'"
            label="Ficheiro .ifc"
            accept=".ifc"
            density="compact"
        ></v-file-input>
        <v-file-input
            v-model="filesDxf"
            v-else
            multiple
            label="Ficheiros .dxf"
            accept=".dxf"
            density="compact"
        />
        <v-row class="mt-2">
            <v-col v-if="canCancel">
                <v-btn
                    block
                    color="error"
                    class="text-center"
                >
                    Cancelar
                </v-btn>
            </v-col>
            <v-col>
                <v-btn
                    type="submit"
                    block
                    color="success"
                    :disabled="!filesIfc && !filesDxf"
                    class="rounded-pill"
                >
                    Submit
                </v-btn>
            </v-col>
        </v-row>
    </v-form>
    <div
        class="d-flex justify-center"
        v-else
    >
        <v-progress-circular
            indeterminate
            color="primary"
            :size="70"
            :width="7"
        />
    </div>
</template>
