<script setup>
import { watch } from "vue";
import { defineProps, computed, ref } from "vue"



const props = defineProps({
  list: {
    type: Array,
    required: true
  }
})

const headers = computed(() => Object.keys(props.list[0]))
const numColsPercent = computed(() => (100 / headers.value.length) + "%")
const page = ref(1);
const maxPerPage = 10;
const numPages = computed(() => Math.ceil(props.list.length / maxPerPage));

watch([page,numPages],()=>{
    if (page.value > numPages.value) {
        page.value = numPages.value
    }
})

</script>
<template>
    <v-table height="20%">
        <thead>
            <tr>
                <th v-for="header in headers" :key="header" class="text-left" >
                    {{ header }}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(item,index) in props.list.slice(maxPerPage*(page-1),maxPerPage*page)" :key="index">
                <td v-for="header in headers" :key="header" class="text-left" > 
                    {{ item[header] }}
                </td>
            </tr>
            </tbody>
    </v-table>
    <v-pagination class="mt-10" v-if="numPages !== 1" v-model="page" :length="numPages"></v-pagination>
</template>

<style scoped>

th{
    width: v-bind(numColsPercent);
    background-color: #e0e0e0;
}
</style>