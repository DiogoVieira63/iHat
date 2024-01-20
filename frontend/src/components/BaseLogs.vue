<script setup lang="ts">
import type { PropType } from 'vue';
import { ref } from 'vue';

const props = defineProps({
    selected : {
        type: Number,
        required: false
    },
    list: {
        type: Array as PropType<Array<string | number>>,
        required: true
    },
    title: {
        type: String,
        required: true
    }
})

const emit = defineEmits(['select'])

const length = ref()


const filterMenu = ref<boolean>(false)

</script>
<template>
    <v-sheet
        width="80%"
        class="mx-auto my-6 rounded-xl pa-2"
    >
        <v-row class="d-flex justify-center my-6">
            <v-spacer></v-spacer>
            <h1 class="text-center text-h4">
                {{ props.selected ? `Alertas do capacete ${props.selected}` : 'Alertas' }}
            </h1>
            <v-spacer></v-spacer>
            <v-menu
                v-model="filterMenu"
                :close-on-content-click="false"
                location="end"
            >
                <template #activator="{ props }">
                    <v-btn
                        variant="flat"
                        color="primary"
                        v-bind="props"
                        icon="mdi-filter"
                        class="mr-8"
                    ></v-btn>
                </template>
                <v-card
                    max-width="200"
                    class="mx-auto"
                >
                    <v-card-text>
                        <h4 class="text-h6">{{ props.title }}</h4>
                        <v-chip-group
                            :model-value="props.selected"
                            @update:model-value="emit('select', $event)"
                            column
                            color="info"
                        >
                            <v-chip
                                v-for="option in props.list"
                                filter
                                variant="outlined"
                                :value="option"
                                :key="option"
                            >
                                {{ option }}
                            </v-chip>
                        </v-chip-group>
                    </v-card-text>
                </v-card>
            </v-menu>
            <v-col cols="12">
                <slot></slot>
            </v-col>
        </v-row>
    </v-sheet>
</template>