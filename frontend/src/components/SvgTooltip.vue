<script setup lang="ts">
import type { ComputedRef, PropType } from 'vue'

export interface Option {
    value: string
    text: string
    icon: string
    disabled?: ComputedRef<boolean>
}

const props = defineProps({
    options: {
        type: Array as PropType<Array<Option>>,
        required: true
    },
    toggle: {
        type: String,
        required: false
    }
})
const emit = defineEmits(['update:modelValue'])
</script>
<template>
    <v-row justify="center">
        <v-btn-toggle
            color="info"
            variant="outlined"
            @update:model-value="emit('update:modelValue', $event)"
            :model-value="props.toggle"
        >
            <v-tooltip
                v-for="option in props.options"
                :key="option.value"
                :text="option.text"
                location="bottom"
            >
                <template v-slot:activator="{ props }">
                    <v-btn
                        v-bind="props"
                        :icon="option.icon"
                        :value="option.value"
                        :disabled="option.disabled?.value"
                    ></v-btn>
                </template>
            </v-tooltip>
        </v-btn-toggle>
    </v-row>
</template>

<style></style>
