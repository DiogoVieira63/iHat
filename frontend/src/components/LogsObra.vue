<script>
  export default {
    data: () => ({
      items: [{message: 'Home', time: '5pm', description: "...", color: "red"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}, {message: 'Home 2', time: '10pm', description: "...", color: "blue"}],
    }),
}
    
</script>

<template>
    <v-infinite-scroll :height="20" side="both" @load="load">
        <v-timeline side="end" align="start">
            <v-timeline-item v-for="item in items"
              :dot-color="item.color"
              size="small"
            >
              <div class="d-flex">
                <strong class="me-4">{{item.time}}</strong>
                <div>
                  <strong>{{item.message}}</strong>
                  <div class="text-caption">
                    {{item.description}}
                  </div>
                </div>
              </div>
            </v-timeline-item>
        </v-timeline>
    </v-infinite-scroll>
</template>