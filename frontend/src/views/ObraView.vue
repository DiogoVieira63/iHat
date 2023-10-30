<script setup>
import Lista from "../components/Lista.vue"
import PageLayout from "../components/PageLayout.vue"
import { computed, ref, watch, onMounted, nextTick } from "vue"
import { useRoute } from 'vue-router'

const route = useRoute()
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
})

</script>

<template>
  <PageLayout>
    <v-row>
        <v-col cols="6" class="pa-6">
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
        </v-col>
        <v-col cols="6" class="pa-6">
            <h1>{{ route.params.id }}</h1>
        </v-col>
    </v-row>
  </PageLayout>
</template>
