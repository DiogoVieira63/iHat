<script setup lang="ts">
import PageLayout from '../components/PageLayout.vue'
import ExampleGraph from '@/components/ExampleGraph.vue'
import LiveData from '@/components/LiveData.vue'
</script>

<template>
    <PageLayout>
        <v-row
            justify="center"
            class="ma-2"
        >
            <v-col
                cols="12"
                lg="6"
            >
                <LiveData />
            </v-col>
            <v-col
                cols="12"
                lg="6"
            >
                <v-card
                    class="mx-auto"
                    prepend-icon="mdi-chart-line"
                >
                    <template v-slot:title> Charts </template>
                    <v-card-text>
                        <ExampleGraph />
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </PageLayout>
</template>
