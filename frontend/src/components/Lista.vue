<script setup>
import { watch } from "vue";
import { computed, ref } from "vue"

import { VDataTable } from 'vuetify/labs/VDataTable'

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
    },
    maxPerPage: {
        type: Number,
        default: 10
    }
})


const numColsPercent = computed(() => (100 / props.headers.length) + "%")
const page = ref(1);
const numPages = computed(() => Math.ceil(props.list.length / props.maxPerPage));
const search = ref("")

watch([page, numPages], () => {
    if (page.value > numPages.value) {
        page.value = numPages.value
    }
})



const rows = computed(() => {
    const startIndex = (page.value - 1) * props.maxPerPage;
    const endIndex = startIndex + props.maxPerPage;
    if (!search.value) {
        return props.list.slice(startIndex, endIndex);
    } else {
        const filtered = props.list.filter(item => {
            return Object.values(item).some(val => {
                return String(val).toLowerCase().includes(search.value.toLowerCase());
            });
        });
        return filtered && filtered.length > 0 ? filtered.slice(startIndex, endIndex) : [];
    }
});

const headersDataTable = computed(() => {
    return props.headers.map(header => {
        return {
            text: header,
            key: header,
            sortable: true
        }
    })
})



</script>
<template>
    <v-row>
        <v-col cols="12" xl="2" lg="3" md="6" class="text-center">
            <slot name="tabs"></slot>
        </v-col>
        <v-spacer></v-spacer>
        <v-col>
            <v-text-field class="mb-1" density="compact" variant="solo" label="Search" v-model="search"
                append-inner-icon="mdi-magnify" single-line hide-details></v-text-field>
        </v-col>
        <v-col cols="6" md="1" align-self="end">
            <slot name="add"></slot>
        </v-col>
    </v-row>    
    <!--v-data-table hover v-if="rows.length > 0" :headers="headersDataTable" :items="props.list" item-value="id">
        <template v-slot:item="{ item }">
            <tr>
            <slot name="row" :row="item" :headers="headers">
                </slot>
            </tr>
        </template>
    </v-data-table-->
    <v-table hover v-if="rows.length > 0" height="55vh">
        <thead>
            <tr>
                <th v-for="header in props.headers" :key="header" class="text-left">
                    {{ header }}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(row, rowIndex) in rows" :key="rowIndex">
                <slot name="row" :row="row" :headers="headers">
                </slot>
            </tr>
        </tbody>
    </v-table>
    <v-alert v-else dense type="info">No results found</v-alert>
    <v-pagination class="mt-5" v-if="numPages !== 1" v-model="page" :length="numPages"></v-pagination>
</template>

<style scoped>
th {
    width: v-bind(numColsPercent);
    background-color: #e0e0e0;
}
</style>