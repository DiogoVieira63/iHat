<script setup>
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch, onMounted, nextTick } from "vue"
import { useRoute, useRouter } from 'vue-router'
import RowObra from "@/components/RowObra.vue"
import MapEditor from "@/components/MapEditor.vue"

const route = useRoute()
const router = useRouter()
const list = ref([])
const headers = ["Id", "Estado", "Actions"]
const title = ref("Nome da Obra " + route.params.id)
const isEditing = ref(false)
const textField = ref(null)

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
    textField.value = ref(null)
    nextTick(() => {
      textField.value.value = $refs.textField 
    })
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



</script>
<template>
  <PageLayout>
      <v-row class="mt-2">
          <v-col cols="6" class="px-16">
            <v-row align="center" justify="start">
              <v-col cols="auto">
                <h1 v-if="!isEditing">{{ title }}</h1>
                <v-text-field
                  v-else
                  v-model="title"
                  dense
                  @keydown.enter="saveTitle"
                  ref="textField"
                  style="width: 300px;"
                ></v-text-field>
              </v-col>
              <v-col cols="auto">
                <v-btn density="compact" icon="mdi-pencil" @click="toggleEditing"></v-btn>
              </v-col>
            </v-row>
            <map-editor />
          </v-col>
          <v-col cols="12" md="6" class="pa-16">
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
