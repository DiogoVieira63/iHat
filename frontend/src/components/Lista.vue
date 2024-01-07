<script setup lang="ts">
import { watch } from 'vue'
import { computed, ref } from 'vue'
import type { Header } from '@/interfaces'
import type { PropType } from 'vue'

const props = defineProps({
    list: {
        type: Array as PropType<Array<{ [id: string]: any }>>,
        required: true
    },
    headers: {
        type: Array as PropType<Array<Header>>,
        required: true
    },
    selected: {
        type: Object as PropType<Record<string,number | string | undefined>>,
        default: () => {}
    }
})

interface Sort {
    column: string | null
    direction: string | null
}
const sort = ref<Sort>({ column: null, direction: null })

const maxPerPage = ref<number>(10)
const page = ref<number>(1)
const search = ref<string>('')
const filterMenu = ref<boolean>(false)

// Filtering

const filterName = (key: string) => {
    const header = props.headers.find((header) => header.key === key)
    return header ? header.name : ''
}

const filterOptions = computed<{ [id: string]: Set<any> }>(() => {
    let options: { [id: string]: Set<any> } = {}
    props.headers.forEach((header: Header) => {
        if (header.params.includes('filter')) {
            options[header.key] = new Set(props.list.map((item) => item[header.key]))
        }
    })
    return options
})

const resetFilter = () => {
    return Object.fromEntries(Object.keys(filterOptions.value).map((key) => [key, []]))
}

const filter = ref<{ [key: string]: string[] }>(resetFilter())

const filterFunc = (row: { [id: string]: string }) => {
    let result = true
    for (let header in filter.value) {
        if (filter.value[header].length > 0 && !filter.value[header].includes(row[header])) {
            result = false
        }
    }
    return result
}

const rowsFilter = computed(() => {
    let filtered = props.list.filter(filterFunc)
    if (!search.value) {
        return filtered
    } else {
        filtered = filtered.filter((item) => {
            return Object.values(item).some((val) => {
                return String(val).toLowerCase().includes(search.value.toLowerCase())
            })
        })
        return filtered
    }
})

// Sorting

const rowsSorted = computed(() => {
    if (sort.value.column) {
        const column = sort.value.column as string
        const rowsCopy = [...rowsFilter.value]
        return rowsCopy.sort((a, b) => {
            if (a[column] > b[column]) {
                return sort.value.direction === 'asc' ? 1 : -1
            } else if (a[column] < b[column]) {
                return sort.value.direction === 'asc' ? -1 : 1
            }
            return 0
        })
    }
    return rowsFilter.value
})

// Pagination

const rowsPage = computed(() => {
    const startIndex = (page.value - 1) * maxPerPage.value
    const endIndex = startIndex + maxPerPage.value
    return rowsSorted.value && rowsSorted.value.length > 0
        ? rowsSorted.value.slice(startIndex, endIndex)
        : []
})

const numPages = computed(() => {
    return rowsFilter.value.length > 0 ? Math.ceil(rowsFilter.value.length / maxPerPage.value) : 1
})

const iconSort = (header: string) => {
    if (sort.value.column === header) {
        return sort.value.direction === 'asc' ? 'mdi-arrow-up' : 'mdi-arrow-down'
    }
    return 'mdi-arrow-up'
}

const hasFilters = computed(() => {
    return Object.keys(filterOptions.value).length > 0
})

const hasElements = computed(() =>{
    return rowsPage.value.length > 0; 
})

const selectSort = (header: string) => {
    if (sort.value.column === header) {
        sort.value.direction = sort.value.direction === 'asc' ? 'desc' : 'asc'
    } else {
        sort.value.column = header
        sort.value.direction = 'desc'
    }
}

watch([page, numPages], () => {
    if (page.value > numPages.value) {
        page.value = numPages.value
    }
})

watch(
    () => filterOptions.value,
    (newValue, oldValue) => {
        for (let key in newValue) {
            if (
                !oldValue[key] ||
                Array.from(newValue[key]).toString() !== Array.from(oldValue[key]).toString()
            ) {
                filter.value = resetFilter()
                break
            }
        }
    }
)

watch(
    () => props.selected,
    (newValue) => {
        if (newValue['key'] && newValue['value']) {
            const index = rowsSorted.value.findIndex((row) => row[String(newValue['key'])] == newValue['value'])
            if (index >= 0) {
                page.value = Math.ceil((index + 1) / maxPerPage.value)
            }
        }
    }
)


const rowSelected = (row: { [id: string]: string }) => {
    if (props.selected && props.selected['key'] && props.selected['value']) {
        if(row[props.selected['key']] == props.selected['value']) 
            return 'bg-grey-lighten-2'
    }
    return ''
}


</script>
<template>
    <v-toolbar class="rounded-t-xl pr-2">
        <slot name="tabs"></slot>
        <v-spacer></v-spacer>
        <v-menu
            v-if="hasFilters && list.length > 0"
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
                    class="mr-4"
                ></v-btn>
            </template>
            <v-card
                max-width="200"
                class="mx-auto"
            >
                <v-card-text>
                    <div
                        v-for="(value, key) in filterOptions"
                        :key="key"
                    >
                        <h2 class="text-h6">{{ filterName(String(key)) }}</h2>
                        <v-chip-group
                            v-model="filter[key]"
                            column
                            multiple
                            color="info"
                        >
                            <v-chip
                                v-for="option in value"
                                filter
                                variant="outlined"
                                :value="option"
                                :key="option"
                            >
                                {{ option }}
                            </v-chip>
                        </v-chip-group>
                        <v-divider />
                    </div>
                </v-card-text>
            </v-card>
        </v-menu>
        <v-responsive 
            max-width="400"
            v-if="list.length > 0"
        >
            <v-text-field
                variant="outlined"
                label="Search"
                v-model="search"
                append-inner-icon="mdi-magnify"
                single-line
                hide-details
                class="mr-4"
                rounded="xl"
            ></v-text-field>
        </v-responsive>
        <div style="flex-basis: 5%">
            <slot name="add"></slot>
        </div>
    </v-toolbar>
    <v-table
        hover
        v-if="hasElements"
        height="60vh"
        fixed-header
        class="border-md rounded-b-xl"
    >
        <thead>
            <tr>
                <th
                    v-for="header in headers"
                    :key="header.key"
                    class="text-center bg-grey-lighten-2"
                >
                    {{ header.name }}
                    <v-btn
                        v-if="header.params.includes('sort')"
                        :icon="iconSort(header.key)"
                        variant="text"
                        @click="selectSort(header.key)"
                    ></v-btn>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr
                class="text-center"
                :class="rowSelected(row)"
                v-for="(row, rowIndex) in rowsPage"
                :key="rowIndex"
            >
                <slot
                    name="row"
                    :row="row"
                    :headers="props.headers"
                ></slot>
            </tr>
        </tbody>
    </v-table>
    <v-sheet
        v-else
        height="60vh"
        class="d-flex align-center"
        border
        rounded="b-xl"
        width="100%"
    >
        <v-alert
            dense
            type="info"
            class="mx-4 rounded-pill"
            >
            Nenhum elemento encontrado
        </v-alert>
    </v-sheet>
    <v-row class="mt-5">
        <v-spacer />
        <v-col
            cols="5"
            md="7"
        >
            <v-pagination
                variant="flat"
                active-color="primary"
                v-if="numPages !== 1"
                v-model="page"
                :length="numPages"
            ></v-pagination>
        </v-col>
        <v-col
            cols="4"
            md="3"
        >
            <v-select
                v-if="list.length > 10"
                dense
                v-model="maxPerPage"
                label="Elements per Page"
                rounded="t-xl"
                :items="[10, 20, 30, 40, 50]"
            ></v-select>
        </v-col>
    </v-row>
</template>

<style>
table {
    table-layout: fixed;
    width: 100%;
}
</style>
