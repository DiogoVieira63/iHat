<script setup>
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch, onMounted, nextTick } from "vue"
import { useRoute, useRouter } from 'vue-router'
import RowObra from "@/components/RowObra.vue"
import Map from "@/components/Map.vue"
import Confirmation from "@/components/Confirmation.vue"
import Logs from "@/components/LogsObra.vue"
//import MapEditor from "@/components/MapEditor.vue"

const route = useRoute()
const router = useRouter()
const list = ref([])
const headers = { 'id': ['sort'], 'Estado': ['filter', 'sort'], 'Actions': [] }//[{'name': 'Id', 'sort': true,'filter'} , "Estado", "Actions"]
const title = ref("Nome da Obra " + route.params.id)
const isEditing = ref(false)
const textField = ref(null)
const estadoObra = ref("Planeada")
const editEstado = ref(false)
const newEstado = ref("")

const toggleEditing = () => {
  isEditing.value = !isEditing.value
  if (isEditing.value) {
    nextTick(() => {
      textField.value.focus()
    })
  }
}

const saveTitle = () => {
  isEditing.value = false
}

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


function removeCapacete(id) {
  list.value = list.value.filter(item => item.id !== id)
}

const onChangeEstado = (value) => {
  editEstado.value = true
}

const newEstadoPossible = (value, open) => {
  newEstado.value = value
  open()
}

const changeEstado = (value) => {
  if (value) {
    estadoObra.value = newEstado.value
  }
  newEstado.value = ""
}

const filtersHeaders = ["Estado"]

</script>
<template>
  <PageLayout>
    <v-row class="mt-2">
      <v-col cols="12" lg="6" class="px-16">
        <v-row align="center" justify="start">
          <v-col cols="auto" v-bind:offset-lg="4">
            <div class="text-h4 text-lg-h3" v-if="!isEditing">{{ title }}</div>
            <v-text-field v-else v-model="title" dense @keydown.enter="saveTitle" ref="textField"
              style="width: 300px;"></v-text-field>
          </v-col>
          <v-col cols="auto">
            <v-btn density="compact" icon="mdi-pencil" @click="toggleEditing"></v-btn>
          </v-col>
          <Map></Map>
        </v-row>
      </v-col>
      <v-col cols="12" lg="6" class="px-16">
        <v-row>
          <v-spacer></v-spacer>
          <v-col cols="12" lg="6" xl="4">
            <confirmation title="Confirmação" :function="changeEstado">
              <template #button="{ open }">
                <v-select label="Estado da Obra" :items="['Planeada', 'Em Curso', 'Pendente', 'Finalizada', 'Cancelada']"
                  :model-value="estadoObra" @update:model-value="(value) => newEstadoPossible(value, open)">
                </v-select>
              </template>
              <template v-slot:message>
                Tem a certeza que pretende mudar o estado da obra de
                <span class="text-red font-weight-bold">{{ estadoObra }}</span> para <span
                  class="text-green font-weight-bold">{{ newEstado }}</span>?
              </template>
            </confirmation>
          </v-col>
        </v-row>
        <Lista v-if="list.length > 0" :list="list" :headers="headers" :filterHeaders="filtersHeaders">
          <template v-slot:tabs>
            <v-toolbar-title>Lista de Capacetes</v-toolbar-title>
          </template>
          <template #row="{ row }">
            <RowObra :row="row" @removeCapacete="(id) => removeCapacete(id)" />
          </template>
          <template v-slot:add>
            <v-btn color="primary" variant="flat" icon="mdi-plus" rounded="xl" class="ma-1">
            </v-btn>
          </template>
        </Lista>
      </v-col>
    </v-row>
    <v-row align="center" class="my-16"> 
      <v-spacer></v-spacer> 
      <v-col cols="8">
        <Logs/>
      </v-col>
      <v-spacer></v-spacer> 
    </v-row>
  </PageLayout>
</template>
