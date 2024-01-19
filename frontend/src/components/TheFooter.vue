<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const links: { [key: string]: string }= {
    'Home': '/', 
    'About Us': '/about', 
}

const linksImgs: { [key: string]: { url: string; image: string } } = {
    'dtx': {url: "https://www.dtx-colab.pt", image: "/dtx.png"},
    'um': {url: "https://www.eng.uminho.pt/pt", image: "/EEUMLOGO.png"}
}

function navigateToLink(url: string) {
    router.push({ path: url })
}


</script>
<template>
    <v-footer
        rounded
        w-auto
        color="secondary"
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
                :key="value.url"
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
</template>

<style>

</style>