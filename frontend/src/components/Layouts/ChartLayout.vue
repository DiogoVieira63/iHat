<script setup lang="ts">
import { computed, onMounted, ref, useSlots } from 'vue';
const slots = useSlots()
const titles = ref<Array<string>>([])
const toggle = ref<Array<string>>([])
const colors = [
    'red',
    'blue',
    'green',
    'yellow',
    'orange',
    'purple',
    'pink',
    'brown',
    'grey',
    'black'
]

onMounted(() => {
    // iterate slots
    let keys = Object.keys(slots)
    for (let slot of keys) {
        titles.value.push(slot)
        toggle.value.push(slot)
    }
})

const columns = computed(() => {
    let columns: { [key: string]: number } = {}
    //iterate titles dict
    let last = ''
    for (let key of titles.value) {
        if (isActive(key)) {
            columns[key] = 12
            columns[key] = 6
            last = key
        }
    }
    let length = Object.keys(columns).length
    if (length % 2 != 0) {
        columns[last] = 12
    }
    return columns
})

const getColumn = (name: string) => {
    return columns.value[name]
}

const isActive = (name: string) => {
    return toggle.value.includes(name)
}
const selectChart = ref('')
const selectIndex = ref(0)
const dialog = ref(false)

const selected = (name: string, index: number) => {
    dialog.value = true
    selectChart.value = name
    selectIndex.value = index
}
</script>

<template>
    <!-- <v-row
        justify="center"
        class="ma-3"
    >
        <v-btn-toggle
            multiple
            rounded="xl"
            v-model="toggle"
            background-color="primary"
        >
            <v-tooltip
                v-for="(key, index) in titles"
                :text="key"
                location="top"
                :key="key"
            >
                <template v-slot:activator="{ props }">
                    <v-btn
                        v-bind="props"
                        :value="key"
                    >
                        <v-icon
                            :color="colors[index]"
                            :active="isActive(key)"
                            >mdi-checkbox-blank-circle</v-icon
                        >
                    </v-btn>
                </template>
            </v-tooltip>
        </v-btn-toggle>
    </v-row> -->
    <v-row>
        <template
            v-for="(key, index) in titles"
            :key="key"
        >
            <Transition>
                <v-col
                    v-if="isActive(key)"
                    cols="12"
                    :lg="getColumn(key)"
                >
                    <v-card>
                        <v-card-title>
                            <v-row class="my-4">
                                <v-icon :color="colors[index]">mdi-checkbox-blank-circle</v-icon>
                                {{ key }}
                                <v-spacer />
                                <v-btn
                                    icon="mdi-fullscreen"
                                    variant="text"
                                    size="md"
                                    @click="selected(key, index)"
                                />
                            </v-row>
                        </v-card-title>
                        <slot :name="key"></slot>
                    </v-card>
                </v-col>
            </Transition>
        </template>
    </v-row>
    <v-dialog
        v-model="dialog"
        persistent
        fullscreen
    >
        <v-card>
            <v-card-title>
                <v-row class="my-4">
                    <v-icon :color="colors[selectIndex]">mdi-checkbox-blank-circle</v-icon>
                    {{ selectChart }}
                    <v-spacer />
                    <v-btn
                        class="mr-4"
                        icon="mdi-close"
                        variant="text"
                        size="md"
                        @click="dialog = false"
                    />
                </v-row>
            </v-card-title>
            <v-card-text>
                <v-sheet
                    class="mx-auto border-md rounded-xl"
                    max-width="80vw"
                    color="background"
                >
                    <slot :name="selectChart"></slot>
                </v-sheet>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>

<style>
.v-enter-active,
.v-leave-active {
    transition: opacity 1s ease;
}

.v-enter-from,
.v-leave-to {
    opacity: 0;
}
</style>
