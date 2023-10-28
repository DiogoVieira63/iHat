<script setup>
import FormObra from "../components/FormObra.vue"
import FormCapacete from "../components/FormCapacete.vue"
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch } from "vue"

const obras = [{ 'id': '1', 'Nome da obra': 'Obra1', 'Estado': 'Pendente' }, { 'id': '2', 'Nome da obra': 'Obra2', 'Estado': 'Em Curso' }]
const Capacetes = [{ 'id': '1', 'Estado': 'Livre' }, { 'id': '2', 'Estado': 'Em uso' }, { 'id': '3', 'Estado': '<v-btn>BUTTON<v-btn>' }]
const tab = ref("obras")
const formObra = ref(true)
const list = ref(obras)



const headers = computed(() => {
  if (tab.value === "obras") {
    return ["Nome da obra", "Estado"]
  } else if (tab.value === "capacetes") {
    return ["Estado"]
  }
})

watch(tab, (newValue, oldValue) => {
  if (newValue === "obras") {
    list.value = obras
    formObra.value = true
  } else if (newValue === "capacetes") {
    list.value = Capacetes
    formObra.value = false
  }
})



</script>

<template>
  <PageLayout>
    <v-container>
      <v-sheet class="mx-auto" max-width="1500px">
        <Lista v-if="list.length > 0" :list="list" :path="tab" :headers="headers">
          <template v-slot:tabs>
            <v-tabs v-model="tab" class="rounded-t-xl align-start" bg-color="grey lighten-3" color="black"
              align-tabs="center">
              <v-tab value="obras" color="black">Obras</v-tab>
              <v-tab value="capacetes" color="black">Capacetes</v-tab>
            </v-tabs>
          </template>
          <template v-slot:add>
            <FormObra v-if="formObra" class="mb-1" />
            <FormCapacete v-else class="mb-1" />
          </template>
          </Lista>
        <v-alert v-else dense type="info">No results found</v-alert>
      </v-sheet>
    </v-container>
  </PageLayout>
</template>

