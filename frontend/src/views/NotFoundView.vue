<script setup lang="ts">
import PageLayout from '@/components/Layouts/PageLayout.vue'
</script>

<template>
    <PageLayout>
        <v-sheet height="80vh" class="d-flex align-center border" >
            <v-row class="my-auto">
                <v-col cols="12">
                    <h2 class="text-h1 text-center">404</h2>
                </v-col>
                    <v-col cols="12">
                        <h3 class="text-h4 text-center">
                        Oops, a Página não foi encontrada
                    </h3>
                </v-col>
                <v-btn
                    color="primary"
                    @click="$router.push('/')"    
                    class="mx-auto mt-6"
                    >
                    Voltar para a página inicial
                </v-btn>
            </v-row>
        </v-sheet>
    </PageLayout>
</template>
