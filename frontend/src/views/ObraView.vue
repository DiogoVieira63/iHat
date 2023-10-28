<script setup>
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch } from "vue"
import { useRoute } from 'vue-router'

const route = useRoute()

</script>

<template>
  <PageLayout>
    <v-row>
        <v-col cols="6">
            <h1>{{ route.params.id }}</h1>
        </v-col>
        <v-col cols="6">
            <h1>{{ route.params.id }}</h1>
        </v-col>
    </v-row>
  </PageLayout>
</template>

