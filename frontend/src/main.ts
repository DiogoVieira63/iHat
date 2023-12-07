// import { createApp } from "vue";
// // import App from "./App.vue";
// import iHat from "./iHat.vue";
// createApp(iHat).use(router).mount("#app");
import { createApp } from 'vue'
import App from './iHat.vue'
import router from './router'
import '@mdi/font/css/materialdesignicons.css'
// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import VueApexCharts from 'vue3-apexcharts'

const vuetify = createVuetify({
    components,
    directives,
    theme: {
        themes: {
            light: {
                dark: false,
                colors: {
                    primary: '#B38DF7',
                    secondary: '#AAC4FF'
                }
            },
            dark: {
                dark: true,
                colors: {
                    primary: '#B38DF7',
                    secondary: '#AAC4FF'
                }
            }
        }
    }
})

const app = createApp(App)
app.use(VueApexCharts)
app.use(vuetify)
app.use(router)
app.mount('#app')

//createApp(App).use(vuetify).use(router).mount('#app')
