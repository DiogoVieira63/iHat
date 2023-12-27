
<script setup lang="ts">
import { onMounted } from 'vue'

const items = [{message: 'Home', time: '5pm', description: "...", type: "informative" }, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alert"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}, {message: 'Home 2', time: '10pm', description: "...", type: "informative"}, {message: 'Home 2', time: '10pm', description: "...", type: "alertDanger"}]


const getIcon = (type: string) => {
  switch (type) {
    case 'informative':
      return 'mdi-information-variant-circle-outline';
    case 'alert':
      return 'mdi-alert-outline';
    case 'alertDanger':
      return 'mdi-alert-outline';
    default:
      return '';
  }
}

const getColor = (type: string) => {
  if (type==="informative") return "info"
  else if (type==="alert") return "warning"
  else if (type==="alertDanger") return "error"
}

onMounted(() => {

})

</script>

<template>
    <v-infinite-scroll height="35vh" side="both" @load="load" class="ma-12">
        <v-timeline side="both" align="start">
            <v-timeline-item v-for="item in items"
              :dot-color="getColor(item.type)"
              size="large"
              :icon="getIcon(item.type)"
              class="me-4"
            >
              <v-card>
                <v-card-title :class="'bg-' + getColor(item.type)">
                  <h2 class="font-weight-light">
                    {{item.message}}
                  </h2>
                </v-card-title>
                <v-card-text>
                  {{item.description}}
                </v-card-text>
              </v-card>
              <template v-slot:opposite>
                  <div
                    :class="`pt-1 headline font-weight-bold text-${getColor(item.type)}`"
                    v-text="item.time"
                  ></div>
                </template>
            </v-timeline-item>
        </v-timeline>
    </v-infinite-scroll>
 
</template>
