<script>
  export default {
    data: () => ({
      links: ['Home', 'About Us', 'Team', 'Services', 'Blog', 'Contact Us'],
    }),
  }
</script>

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


 <template>
  <v-app>
    <v-app-bar :elevation="3" color="#B38DF7" rounded height="100">
			<v-btn icon><img src="/Hotpot.png" alt="Image" style="width: 80px; height: 80px;"/></v-btn>
			<v-app-bar-title><b>iHat</b></v-app-bar-title>
      <template v-slot:append>
        <v-btn icon="mdi-heart"></v-btn>
        <v-btn icon="mdi-magnify"></v-btn>
        <v-btn icon="mdi-dots-vertical"></v-btn>
      </template>
    </v-app-bar>
    <v-main>
      <slot></slot>
    </v-main>
    <v-footer rounded w-auto color="#B38DF7" class="v-footer--full-width">
      <v-row justify="center" no-gutters class="v-row--full-width">
        <v-btn
          v-for="link in links"
          :key="link"
          color="white"
          variant="text"
          class="mx-2"
          rounded="xl"
        >
          {{ link }}
        </v-btn>
        <v-col class="text-center mt-4" cols="12">
          {{ new Date().getFullYear() }} — <strong>iHat</strong>
        </v-col>
      </v-row>
    </v-footer>
  </v-app>
  
  
</template> 






