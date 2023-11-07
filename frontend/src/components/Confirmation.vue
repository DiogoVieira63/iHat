<script setup>
import { ref } from "vue"

const props = defineProps({
    title: {
        type: String,
        required: true
    },
    function: {
        type: Function,
        required: true
    }
})

const dialog = ref(false)

function callFunction(value) {
    dialog.value = false
    props.function(value)
}

function openDialog() {
    dialog.value = true
}

</script>
<template>
    <v-dialog v-model="dialog" persistent width="auto">
        <template v-slot:activator="{ props }">
            <slot name="button" :prop="props" :open="openDialog" ></slot>
        </template>
        <v-card>
            <v-card-title class="text-h5">
                {{ title }}
            </v-card-title>
            <v-card-text>
                <slot name="message"></slot>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="red" variant="text" @click="callFunction(false)">
                    Cancelar
                </v-btn>
                <v-btn color="green" variant="elevated" @click="callFunction(true)">
                    Confirmar
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

