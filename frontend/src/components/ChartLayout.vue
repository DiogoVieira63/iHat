<script setup>
import { ref } from 'vue';
import { useSlots, computed, onMounted } from 'vue'
const slots = useSlots()
const titles = ref([]);
const toggle = ref([]);
const colors = ["red", "blue", "green", "yellow", "orange", "purple", "pink", "brown", "grey", "black"]

onMounted(() => {
    // iterate slots
    let keys = Object.keys(slots)
    for (let slot of keys) {
        titles.value.push(slot)
        toggle.value.push(slot)
    }
})

const columns = computed(() => {
    let columns = {}
    //iterate titles dict
    let last = ""
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


const getColumn = (name) => {
    return columns.value[name]
}

const isActive = (name) => {
    return toggle.value.includes(name)
}

</script>



<template>
    <v-row justify="center" class="ma-3">
        <v-btn-toggle multiple rounded="xl" v-model="toggle" background-color="primary">
            <v-tooltip v-for="(key, index) in titles" :text="key" location="top">
                <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" :value="key">
                        <v-icon :color="colors[index]" :active="isActive(key)">mdi-checkbox-blank-circle</v-icon>
                    </v-btn>
                </template>
            </v-tooltip>
        </v-btn-toggle>
    </v-row>
    <v-row>
        <template v-for="(key, index) in titles">
            <Transition>
                <v-col v-if="isActive(key)" cols="12" :lg="getColumn(key)">
                    <v-card>
                        <v-card-title>
                            <v-icon :color="colors[index]">mdi-checkbox-blank-circle</v-icon>
                            {{ key }}
                        </v-card-title>
                        <slot :name="key"></slot>
                    </v-card>
                </v-col>
            </Transition>
        </template>
    </v-row>
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



