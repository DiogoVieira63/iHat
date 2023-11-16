<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import Confirmation from '@/components/Confirmation.vue'
import { useDisplay } from 'vuetify'
//
// Destructure only the keys you want to use
const { mdAndDown } = useDisplay()

const page = ref(1)
const edit = ref(false)
const imageUrls = ['/Duplex-0.svg', '/Duplex-1.svg', '/Duplex-2.svg', '/Duplex-3.svg']

const getCurrentImage = computed(() => {
  const currentIndex = page.value - 1
  const validIndex = Math.min(Math.max(currentIndex, 0), imageUrls.length - 1)
  return imageUrls[validIndex]
})

const saveEdit = (confirmation) => {
  if (confirmation) {
    console.log('Save')
  } else {
    console.log('Cancel')
  }
  edit.value = false
}
</script>

<template>
  <template v-for="image in imageUrls">
    <map-editor :active="image === getCurrentImage" :edit="edit" :svg-src="image"></map-editor>
  </template>
  <v-container>
    <v-row>
      <v-spacer />
      <v-col cols="10" md="8">
        <v-pagination v-model="page" :length="imageUrls.length" :total-visible="5"></v-pagination>
      </v-col>
      <v-col cols="2" md="4" class="mt-3">
        <v-btn
          v-if="!edit"
          prepend-icon="$edit"
          variant="tonal"
          color="primary"
          @click="edit = !edit"
        >
          {{ mdAndDown ? '' : 'Editar' }}
        </v-btn>
        <confirmation v-else title="Confirmação" :function="saveEdit">
          <template #button="{ prop }">
            <v-btn v-bind="prop" color="primary" prepend-icon="mdi-content-save">
              {{ mdAndDown ? '' : 'Terminar Edição' }}
            </v-btn>
          </template>
          <template v-slot:message> Pretende guardar a edição do mapa? </template>
        </confirmation>
      </v-col>
      <v-spacer />
    </v-row>
  </v-container>
</template>

<style>
.center {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 70vh;
  /* Optional: Center content vertically within the viewport */
}
</style>
