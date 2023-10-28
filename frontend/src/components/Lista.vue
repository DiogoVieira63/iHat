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
    },
    headers: {
        type: Array,
        required: true
    }

})


const numColsPercent = computed(() => (100 / props.headers.length) + "%")
const page = ref(1);
const maxPerPage = 2;
const numPages = computed(() => Math.ceil(props.list.length / maxPerPage));
const search = ref("")

watch([page, numPages], () => {
    if (page.value > numPages.value) {
        page.value = numPages.value
    }
})

function changePage(id) {
    router.push({ path: `${props.path}/${id}` })
}

const rows = computed(() => {
  if (!search.value) {
    return props.list;
  } else {
    const filtered = props.list.filter(item => {
      return Object.values(item).some(val => {
        return String(val).toLowerCase().includes(search.value.toLowerCase());
      });
    });
    const startIndex = (page.value - 1) * maxPerPage;
    const endIndex = startIndex + maxPerPage;
    return filtered.slice(startIndex, endIndex) && filtered.length > 0 ? filtered : [];
  }
});



</script>
<template>
    <v-row>

        <v-col cols="12" xl="2" lg="3" md="6" class="text-center">
            <slot name="tabs"></slot>
        </v-col>
        <v-spacer></v-spacer>
        <v-col>
            <v-text-field density="compact" variant="solo" label="Search" v-model="search" append-inner-icon="mdi-magnify"
                single-line hide-details></v-text-field>
        </v-col>
        <v-col cols="6" md="1" align-self="end">
            <slot name="add"></slot>
        </v-col>
    </v-row>

    <v-table height="50vh" hover>
        <thead>
            <tr>
                <th v-for="header in props.headers" :key="header" class="text-left">
                    {{ header }}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(row, rowIndex) in rows" :key="rowIndex" @click="changePage(row.id)">
                <td v-for="header in props.headers" :key="header" class="text-left">
                    {{ row[header] }}
                </td>
            </tr>
        </tbody>
    </v-table>
    <v-pagination class="mt-10 " v-if="numPages !== 1" v-model="page" :length="numPages"></v-pagination>
</template>

<style scoped>
th {
    width: v-bind(numColsPercent);
    background-color: #e0e0e0;
}
</style>