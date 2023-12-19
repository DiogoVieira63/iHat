<script setup lang="ts">
import { ref } from 'vue'
import type { PropType } from 'vue';

const props = defineProps({
    title: {
        type: String,
        default: 'Title'
    },
    range: {
        type: Array as PropType<Array<number>>,
        default: () => [-10, 10]
    },
    value: {
        type: Array as PropType<Array<number>>,
        default: () => [0, 0]
    }
})


const emit = defineEmits(['updateValue'])

const updateValue = (value: [number,number]) => {
    emit('updateValue', value)
}

const updatePosition = (value: number, index: number) => {
    const newValue = [...props.value]
    newValue[index] = value
    emit('updateValue', newValue)
}

const rules = [
    (v: number) => v >= props.value[0] || 'Inválido',
    (v: number) => v <= props.value[1] || 'Inválido',
    (v: number) => v >= props.range[0] && v <= props.range[1] || 'Fora do Range',
]

</script>
<template>
    <v-card height="fit-content">
        <v-card-title>
            <h4 class="text-center">{{ props.title }}</h4>
        </v-card-title>
        <v-card-text>
            <v-range-slider :model-value="props.value" @update:model-value="updateValue" thumb-label="always" class="mt-16" step="0.01" :min="props.range[0]" :max="props.range[1]" >
            </v-range-slider>
            <v-row>
                <v-col cols="6" >
                    <v-text-field
                        :model-value="props.value[0]"
                        @update:model-value="updatePosition(Number($event),0)"
                        single-line
                        step="0.01"
                        type="number"
                        variant="outlined"
                        density="compact"
                        label="Min"
                        :rules="rules"
                      ></v-text-field>
                </v-col>
                <v-col cols="6" >
                    <v-text-field
                        :model-value="props.value[1]"
                        @update:model-value="updatePosition(Number($event),1)"
                        single-line
                        step="0.01"
                        type="number"
                        variant="outlined"
                        density="compact"
                        label="Max"
                        :rules="rules"
                      ></v-text-field>
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

<style></style>