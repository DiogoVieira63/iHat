<script setup>
import { watch } from "vue";
import { computed, ref } from "vue"

import { useRouter } from 'vue-router'

const router = useRouter()
const props = defineProps({
  list: {
    type: Array,
    required: true
  },
    path: {
        type: String,
        required: true
    }
})


const headers = computed(() => Object.keys(props.list[0]))
const numColsPercent = computed(() => (100 / headers.value.length) + "%")
const page = ref(1);
const maxPerPage = 3;
const numPages = computed(() => Math.ceil(props.list.length / maxPerPage));

watch([page,numPages],()=>{
    if (page.value > numPages.value) {
        page.value = numPages.value
    }
})

function changePage(id) {
    router.push({ path: `${props.path}/${id}` })
}

const rows = computed(() => {
  const startIndex = (page.value - 1) * maxPerPage;
  const endIndex = startIndex + maxPerPage;
  return props.list.slice(startIndex,endIndex);
});


</script>
<template>
    <v-table height="50vh" hover>
        <thead>
            <tr>
                <th v-for="header in headers" :key="header" class="text-left" >
                    {{ header }}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(row, rowIndex) in rows" :key="rowIndex" @click="changePage(row.id)">
                <td v-for="header in headers" :key="header" class="text-left" > 
                    {{ row[header] }}
                </td>
            </tr>
            </tbody>
    </v-table>
    <v-pagination class="mt-10 " v-if="numPages !== 1" v-model="page" :length="numPages"></v-pagination>
</template>

<style scoped>

th{
    width: v-bind(numColsPercent);
    background-color: #e0e0e0;
}
</style>