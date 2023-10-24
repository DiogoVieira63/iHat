<script setup>
import FormObra from "../components/FormObra.vue"
import Lista from "../components/Lista.vue"
import { reactive, ref, watch } from "vue"

const obras = [{ 'Nome da obra': 'Obra1', 'Estado': 'Pendente' }, { 'Nome da obra': 'Obra2', 'Estado': 'Em Curso' }]
const Capacetes = [{ 'Id': '1', 'Estado': 'Pendente'}, { 'Id': 'Obra2', 'Estado': 'Em Curso' }]
const tab = ref("Obras")
const list = ref(obras)
const formObra = ref(true)

watch(tab, (newValue, oldValue) => {
  if (newValue === "Obras") {
    list.value = obras
    formObra.value = true
  } else if (newValue === "Capacetes") {
    list.value = Capacetes
    formObra.value = false
  }
})

</script>

<template>
  <v-container>
    <v-sheet class="mx-auto" max-width="1500px">
      <v-row>
        <v-col cols="12" xl="2" lg="3" md="6" class="text-center">
          <v-tabs v-model="tab" class="rounded-t-xl align-start" bg-color="grey lighten-3" color="black"
            align-tabs="center">
            <v-tab value="Obras" color="black">Obras</v-tab>
            <v-tab value="Capacetes" color="black">Capacetes</v-tab>
          </v-tabs>
        </v-col>
        <v-spacer></v-spacer>
        <v-col cols="1" align-self="end">
        <FormObra v-if="formObra" class="mb-1"/>
        <!--FormCapacete v-else/-->
        </v-col>
      </v-row>
      <Lista :list="list" />
    </v-sheet>
  </v-container>
</template>

