<script setup lang="ts">
import { ref } from 'vue'

const tipo = ref('ifc')

const filesIfc = ref<File[] | undefined>(undefined)
const filesDxf = ref<File[] | undefined>(undefined)

const submit = (values: any) => {
    if (tipo.value == 'ifc') {
        console.log(filesIfc.value)
        console.log(typeof filesIfc.value)
    } else {
        console.log(filesDxf.value)
        console.log(typeof filesDxf.value)
    }
    console.log(JSON.stringify(values, null, 2))
}
</script>

<template>
    <v-form @submit.prevent="submit">
        <v-radio-group v-model="tipo" inline class="d-flex justify-center my-5">
            <v-radio label="IFC" value="ifc"></v-radio>
            <v-radio label="DXF" value="dxf"></v-radio>
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
        <v-btn type="submit" block class="mt-2" color="primary">Submit</v-btn>
    </v-form>
</template>
