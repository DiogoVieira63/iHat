<script setup>
import FormObra from "../components/FormObra.vue"

import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch } from "vue"

const obras = [{ 'id':'1','Nome da obra': 'Obra1', 'Estado': 'Pendente' }, { 'id':'2','Nome da obra': 'Obra2', 'Estado': 'Em Curso' }]
const Capacetes = [{ 'id': '1', 'Estado': 'Livre' }, { 'id': '2', 'Estado': 'Em uso' }, { 'id': '3', 'Estado': 'Ocupado' }]
const tab = ref("obras")
const list = ref(obras)
const formObra = ref(true)
const search = ref("")

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

const filteredList = computed(() => {
  if (!search.value) {
    return list.value;
  } else {
    const filtered = list.value.filter(item => {
      return Object.values(item).some(val => {
        return String(val).toLowerCase().includes(search.value.toLowerCase());
      });
    });
    return filtered && filtered.length > 0 ? filtered : [];
  }
});


</script>

<template>

  <PageLayout>
    <v-container>
      <v-sheet class="mx-auto" max-width="1500px">
        <v-row>
          <v-col cols="12" xl="2" lg="3" md="6" class="text-center">
            <v-tabs v-model="tab" class="rounded-t-xl align-start" bg-color="grey lighten-3" color="black"
              align-tabs="center">
              <v-tab value="obras" color="black">Obras</v-tab>
              <v-tab value="capacetes" color="black">Capacetes</v-tab>
            </v-tabs>
          </v-col>
          <v-spacer></v-spacer>
          <v-col>
            <v-text-field density="compact" variant="solo" label="Search" v-model="search"
              append-inner-icon="mdi-magnify" single-line hide-details></v-text-field>
          </v-col>
          <v-col cols="6" md="1" align-self="end">
            <FormObra v-if="formObra" class="mb-1" />
            <FormCapacete v-else/>
          </v-col>
        </v-row>
        <Lista v-if="filteredList.length > 0" :list="filteredList" :path="tab" :headers="headers"/>
        <v-alert v-else dense type="info">No results found</v-alert>
      </v-sheet>
    </v-container>
  </PageLayout>
</template>

