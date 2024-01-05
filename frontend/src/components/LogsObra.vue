
<script setup lang="ts">
import { onMounted, type PropType } from 'vue'
import type { Log } from '@/interfaces';

const props = defineProps({
    logs: {
        type: Array as PropType<Array<Log>>,
        required: true
    }
})

const items = [{message: 'Home', time: '5pm', description: "...", type: "informative" }, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}]


const getIcon = (type: string) => {
  switch (type) {
    // case 'informative':
    //   return 'mdi-information-variant-circle-outline';
    case 'Alerta':
      return 'mdi-alert-outline';
    case 'Grave':
      return 'mdi-alert-outline';
    default:
      return '';
  }
}

const getColor = (type: string) => {
  // if (type==="informative") return "info" else 
  if (type==="Alerta") return "warning"
  else if (type==="Grave") return "error"
}

onMounted(() => {

})

</script>

<template>
    <v-container v-if="props.logs.length > 0">
      <v-infinite-scroll height="35vh" side="end" class="ma-12">
          <v-timeline align="start">
              <v-timeline-item v-for="log in props.logs"
                :dot-color="getColor(log.type)"
                size="large"
                :icon="getIcon(log.type)"
                class="me-4"
              >
                <v-card>
                  <v-card-title :class="'bg-' + getColor(log.type)">
                    <h2 class="font-weight-light">
                      {{log.type}}
                    </h2>
                  </v-card-title>
                  <v-card-text>
                    <h3>{{log.mensagem}}</h3>
                  </v-card-text>
                </v-card>
                <template v-slot:opposite>
                    <div
                      :class="`pt-1 headline font-weight-bold text-${getColor(log.type)}`"
                      v-text="log.timestamp"
                    ></div>
                  </template>
              </v-timeline-item>
          </v-timeline>
      </v-infinite-scroll>
    </v-container>
    <v-container v-else>
      <v-sheet
        height="35vh"
        width="400px"
        class="d-flex align-center mx-auto" 
        rounded="b-xl"
      >
        <v-alert
          dense
          type="info"
          class="mx-4 rounded-pill"
        >
          Não existem logs para esta obra
        </v-alert>
      </v-sheet>
  </v-container>
 
</template>
