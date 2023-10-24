// import { createApp } from "vue";
// // import App from "./App.vue";
// import iHat from "./iHat.vue";
// createApp(iHat).use(router).mount("#app");
import { createApp } from "vue";
import App from "./iHat.vue";
import router from "./router";
import "@mdi/font/css/materialdesignicons.css";
// Vuetify
import "vuetify/styles";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    themes: {
      light: {
        dark: false,
        colors: {
          primary: "#B38DF7", 
        }
      },
    },
  },
});

createApp(App).use(vuetify).use(router).mount("#app");
