<script setup>
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch, onMounted } from "vue"
import { useRoute, useRouter } from 'vue-router'
import RowObra from "@/components/RowObra.vue"


const route = useRoute()
const router = useRouter()
const list = ref([])
const headers = ["Id", "Estado", "Actions"]

onMounted(() => {
    for (let i = 0; i < 30; i++) {
        // random Estado between "Livre" and "Em uso" and "Não Operacional"
        const randomEstado = Math.floor(Math.random() * 3)
        let estado = ""
        if (randomEstado === 0) {
            estado = "Livre"
        } else if (randomEstado === 1) {
            estado = "Em uso"
        } else {
            estado = "Não Operacional"
        }
        list.value.push({ 'id': i, 'Estado': estado })
    }
})


function changePage(id) {
    router.push({ path: `${props.path}/${id}` })
}

function removeCapacete(id) {
    list.value = list.value.filter(item => item.id !== id)
}



</script>
<template>
    <PageLayout>
        <v-row class="mt-2">
            <v-col cols="6">
                <h1>{{ route.params.id }}</h1>
            </v-col>
            <v-col cols="12" md="6">
                <Lista v-if="list.length > 0" :list="list" path="/capacetes" :headers="headers" :maxPerPage="8">
                    <template v-slot:tabs>
                        <h3>Capacetes</h3>
                    </template>
                    <template #row="{ row }">
                        <RowObra :row="row" @removeCapacete="(id) => removeCapacete(id)" />
                    </template>
                    <template v-slot:add>
                        <v-btn icon color="primary">
                            <v-icon color="black">mdi-plus</v-icon>
                        </v-btn>
                    </template>
                </Lista>
            </v-col>
        </v-row>
    </PageLayout>
</template>

