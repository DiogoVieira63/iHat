<script setup lang="ts">
import type { PropType } from 'vue';
import { ref, watch} from 'vue';

const props = defineProps({
    range: {
        type: Array as PropType<Array<number>>,
        default: () => [-10, 10]
    },
    value: {
        type: Array as PropType<Array<number>>,
        default: () => [0, 0]
    },
    tipo: {
        type: String,
        default: 'Variável'
    },
    step: {
        type: Number,
        default: 0.01
    }
})

const emit = defineEmits(['updateValue'])
const min = ref(props.value[0])
const max = ref(props.value[1])
const val = ref(props.value[0])

watch(min, (value) => {
    updatePosition(value, 0)
})

watch(max, (value) => {
    updatePosition(value, 1)
})

watch(val, (value) => {
    updateValue([value, value])
})

const updateValue = (value: [number, number]) => {
    emit('updateValue', value)
}

const updateMinMax = (value: [number, number]) => {
    min.value = value[0]
    max.value = value[1]
}

const updatePosition = (value: number, index: number) => {
    const newValue = [...props.value]
    newValue[index] = value
    emit('updateValue', newValue)
}

const rules = {
    min: (v: number) => v >= props.value[0] || 'Inválido',
    max: (v: number) => v <= props.value[1] || 'Inválido',
    range: (v: number) =>
        (v >= props.range[0] && v <= props.range[1]) || `Fora do Range:[${props.range}]`
}
</script>
<template>
    <template v-if="tipo == 'Variável'">
        <v-range-slider
            :model-value="props.value"
            @update:model-value="updateMinMax"
            thumb-label="always"
            class="mt-5"
            :step="props.step"
            :min="props.range[0]"
            :max="props.range[1]"
        >
        </v-range-slider>
        <v-row>
            <v-col cols="6">
                <v-text-field
                    v-model="min"
                    single-line
                    :step="props.step"
                    type="number"
                    variant="outlined"
                    density="compact"
                    label="Min"
                    :rules="[rules.max, rules.range]"
                ></v-text-field>
            </v-col>
            <v-col cols="6">
                <v-text-field
                    v-model="max"
                    single-line
                    :step="props.step"
                    type="number"
                    variant="outlined"
                    density="compact"
                    label="Max"
                    :rules="[rules.min, rules.range]"
                ></v-text-field>
            </v-col>
        </v-row>
    </template>
    <template v-else>
        <v-slider
            class="mt-5"
            v-model="val"
            :min="props.range[0]"
            :max="props.range[1]"
            :step="props.step"
            thumb-label="always"
        ></v-slider>
        <v-text-field
            v-model="val"
            single-line
            :step="props.step"
            type="number"
            variant="outlined"
            density="compact"
            label="Valor"
            :rules="[rules.range]"
        ></v-text-field>
    </template>
</template>

<style></style>
