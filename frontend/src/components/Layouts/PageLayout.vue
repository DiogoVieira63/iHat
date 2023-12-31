<script setup lang="ts">
import { useTheme } from 'vuetify'
import { useRouter } from 'vue-router'

const links: { [key: string]: string }= {
    'Home': '/', 
    'About Us': '/about', 
    'Team': '/team',  
}

const linksImgs: { [key: string]: { url: string; image: string } } = {
    'dtx': {url: "https://www.dtx-colab.pt", image: "/dtx.png"},
    'um': {url: "https://www.eng.uminho.pt/pt", image: "/EEUMLOGO.png"}
}

const theme = useTheme()
const router = useRouter()

function toggleTheme() {
    theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark'
}

function navigateToLink(url: string) {
    router.push({ path: url })
}

</script>

<template>
    <v-app>
        <v-app-bar
            :elevation="3"
            color="primary"
            rounded
            height="80"
        >
            <v-app-bar-nav-icon @click="router.push('/')">
                <v-img
                    src="/Hotpot.ico"
                    alt="Image"
                    width="60"
                ></v-img>
            </v-app-bar-nav-icon>
            <v-app-bar-title><b>iHat</b></v-app-bar-title>
            <template v-slot:append>
                <v-btn
                    icon="mdi-theme-light-dark"
                    @click="toggleTheme"
                ></v-btn>
                <v-btn icon="mdi-heart"></v-btn>
                <v-btn icon="mdi-magnify"></v-btn>
                <v-btn icon="mdi-dots-vertical"></v-btn>
            </template>
        </v-app-bar>
        <v-main>
            <slot></slot>
        </v-main>
        <v-footer
            rounded
            w-auto
            color="primary"
            class="flex-column"

        >
            <v-row
                justify="center"
                no-gutters
                class="py-1"
            >
                <v-btn
                    v-for="(value, key) in links"
                    :key="key"
                    color="white"
                    variant="text"
                    class="mx-8"
                    rounded="xl"
                    @click="navigateToLink(value)"
                >
                    {{ key }}
                </v-btn>                
            </v-row>
            <v-row
                justify="center"
                no-gutters
                class="py-1"
            >
                <v-hover 
                    v-slot="{ isHovering, props }"
                    close-delay="200"
                    v-for="value in linksImgs"
                >
                    <v-card
                        color="primary"
                        width="100" height="50"
                        class="mx-8"
                        :elevation="isHovering ? 10 : 0"
                        :class="{ 'on-hover': isHovering }"
                        v-bind="props"
                    >
                        <a 
                            :href="value.url"
                        >
                            <v-img :src="value.image" width="100" height="50" ></v-img>
                        </a>
                    </v-card>
                </v-hover>
             </v-row>
             <v-col
                    class="text-center mt-1"
                >
                    {{ new Date().getFullYear() }} — <strong>iHat</strong>
             </v-col>
        </v-footer>
    </v-app>
</template>

<style scoped>
.v-footer--full-width {
    width: 100%;
    left: 0;
    right: 0;
    position: absolute;
    bottom: 0;
}

.v-row--full-width {
    width: 100%;
    margin-left: 0;
    margin-right: 0;
}
</style>
