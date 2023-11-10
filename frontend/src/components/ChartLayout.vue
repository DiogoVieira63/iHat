<script setup>
import { useSlots, computed} from 'vue'
const items = ['item1', 'item2','item3']

const slots = useSlots()

const slotsLength = () => {
    return Object.keys(slots).map(key => slots[key]).length
}

const columns = computed(() => {
    let columns = []
    for (let i = 0; i < slotsLength(); i++) {
        columns.push(6)
    }
    if (slotsLength() % 2 != 0){
        columns[slotsLength()] = 12
    }
    return columns
})


const elemName = (index) => {
    return "item" + (index)
}


</script>



<template>
    {{ slotsLength() }}
    <v-row>
    <template v-for="i in slotsLength()">
            <v-col cols="12" :lg="columns[i-1]">
                <slot :name="elemName(i)">Slot</slot>
            </v-col>
        </template>
    </v-row>
</template> 



<style scoped>
</style>



