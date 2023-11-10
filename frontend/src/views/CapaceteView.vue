<script setup>
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch, onMounted } from "vue"
import { useRoute, useRouter } from 'vue-router'
import ExampleGraph from "@/components/ExampleGraph.vue";


const options = {
    chart: {
        id: 'vuechart-example'
    },
    xaxis: {
        categories: [1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998]
    },
    stroke: {
        curve: 'smooth'
    },
}
const series = [{
    name: 'series-1',
    data: [30, 40, 45, 50, 49, 60, 70, 91]
}]

</script>

<template>
    <PageLayout>
        <v-row justify="center" class="ma-2">
            <v-col cols="12" lg="6">
                <v-card class="mx-auto" prepend-icon="mdi-home" height="80vh">
                    <template v-slot:title>
                        This is a title
                    </template>

                    <v-card-text>
                        This is content
                    </v-card-text>
                </v-card>
            </v-col>
            <v-col cols="12" lg="6">
                <v-card class="mx-auto" prepend-icon="mdi-home">
                    <template v-slot:title>
                        This is a title
                    </template>
                    <v-card-text>
                        <ExampleGraph/> 
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </PageLayout>
</template>

