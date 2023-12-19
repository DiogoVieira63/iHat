<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import ObraLayout from '@/components/Layouts/ObraLayout.vue'
import { useRouter } from 'vue-router'
import SimuladorInput from '@/components/SimuladorInput.vue'


export interface Capacete {
    position : {x: number, y: number},
    key: number,
}
const router = useRouter()
const page = ref(1)
const edit = ref(false)
const imageUrls: Array<string> = ['/Duplex1.svg', '/Duplex2.svg']
const range = ref([-10, 10])
const selected = ref(0)
const tipo = ref('Variável')

const getCurrentImage = computed(() => {
    const currentIndex = page.value - 1
    const validIndex = Math.min(Math.max(currentIndex, 0), imageUrls.length - 1)
    return imageUrls[validIndex]
})

const capacetes = ref<Array<Capacete>>([])

const updateCapacete = (capacete: Capacete) => {
    capacetes.value = capacetes.value.map((item) => {
        if (item.key === capacete.key) {
            return capacete
        }
        return item
    })
}

const goToObraPage = () => {
    router.push("/obras/" + router.currentRoute.value.params.id)
}

const inputs = ref([
    {
        title: "Temperatura Corpural",
        range: [35, 42],
        value: [36.5, 37.5]
    },
    {
        title: "Ritmo Cardíaco",
        range: [80, 200],
        value: [80, 100]
    },
    {
        title: "Probabilidade de Queda",
        range: [0, 1],
        value: [0.5, 0.5]
    },
    {
        title: "Proximidade",
        range: [0, 200],
        value: [0, 0]
    },
    {
        title: "Gases Tóxicos (Monóxido de Carbono)",
        range: [0, 1],
        value: [0, 0]
    },
    {
        title: "Gases Tóxicos (Dióxido de Carbono)",
        range: [0, 1],
        value: [0, 0]
    },
    {
        title: "Posição do Capacete (X)",
        range: [0, 1],
        value: [0, 0]
    },
    {
        title: "Posição do Capacete (Y)",
        range: [0, 1],
        value: [0, 0]
    }
])

</script>
<template>
    <page-layout>
        <ObraLayout>
            <template #map>
                <template v-for="image in imageUrls" :key="image">
                    <map-editor :active="image === getCurrentImage" :edit="true" :svg-src="image"
                        :capacetes-position="capacetes"
                        :capacete-selected="selected"
                        @addCapacete="capacetes.push($event)"   
                        @selectCapacete="selected = $event"
                        @update::capacete="updateCapacete($event)"
                        options="Simulador"></map-editor>
                </template>
                <v-row class="d-flex justify-center mt-5">
                    <v-pagination v-model="page" :length="imageUrls.length" :total-visible="5" />
                </v-row>
            </template>
            <template #content>
                <v-row class="d-flex justify-end my-2">
                    <v-btn
                        rounded="xl"
                        size="large"
                        variant="flat"
                        color="primary"
                        @click="goToObraPage" >
                        Página Obra
                    </v-btn>
                </v-row>
                <v-container>
                    <v-card height="65vh" style="overflow: auto;">
                        <v-card-title class="text-center text-h4 my-4">
                            Simulador
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" md="6">
                                    <v-btn 
                                        block 
                                        value="Constante" 
                                        :color="tipo=='Constante' ? 'primary' : 'defualt'"
                                        @click="tipo = 'Constante'"
                                    >
                                        Constante
                                    </v-btn>
                                </v-col>
                                <v-col cols="12" md="6">
                                    <v-btn 
                                        block 
                                        value="Variável" 
                                        :color="tipo=='Variável' ? 'primary' : 'value'"
                                        @click="tipo = 'Variável'"
                                    >
                                        Variável
                                    </v-btn>
                                </v-col>
                                <v-col cols="12" md="6" v-for="input in inputs" :key="input.title"  >
                                    <SimuladorInput 
                                        @updateValue="input.value = $event"
                                        :title="input.title" 
                                        :range="input.range" 
                                        :value="input.value"
                                    />
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-card>
                </v-container>
            </template>
        </ObraLayout>
    </page-layout>
</template>

<style>
</style>
