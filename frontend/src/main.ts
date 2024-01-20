import '@mdi/font/css/materialdesignicons.css'
import { createApp } from 'vue'
import App from './iHat.vue'
import router from './router'
import { createPinia } from 'pinia'
import VueApexCharts from 'vue3-apexcharts'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import 'vuetify/styles'

const pinia = createPinia()

const vuetify = createVuetify({
    components,
    directives,
    theme: {
        themes: {
            light: {
                dark: false,
                colors: {
                    primary: '#D75958',
                    secondary: '#848283',
                    info:'#435395',
                    surface:'#F5F5F5'
                }
            },
            dark: {
                dark: true,
                colors: {
                    primary: '#D75958',
                    secondary: '#848283',
                    info:'#435395'
                }
            }
        }
    }
})

// var corsOptions = {
//     origin: "http://localhost:3000"
//   };

const app = createApp(App)
app.use(VueApexCharts)
app.use(vuetify)
app.use(router)
app.use(pinia)
app.mount('#app')

//createApp(App).use(vuetify).use(router).mount('#app')
