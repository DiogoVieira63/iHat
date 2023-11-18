<script>
  export default {
    data: () => ({
      items: [{message: 'Home', time: '5pm', description: "...", color: "red"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}],
    }),
}
    
</script>

<template>
    <v-infinite-scroll :height="200" side="both" @load="load">
        <v-timeline side="both" align="start">
            <v-timeline-item v-for="item in items"
              :dot-color="item.color"
              size="large"
              icon="mdi-alert-outline"
              class="me-4"
            >
              <v-card>
                <v-card-title :class="'bg-' + item.color">
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
                    :class="`pt-1 headline font-weight-bold text-${item.color}`"
                    v-text="item.time"
                  ></div>
                </template>
            </v-timeline-item>
        </v-timeline>
    </v-infinite-scroll>
</template>