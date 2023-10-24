<script setup>



</script>


 <template>
  <v-app>
    <v-app-bar :elevation="3" color="primary" rounded height="100">
			<v-btn icon><img src="/Hotpot.png" alt="Image" style="width: 80px; height: 80px;"/></v-btn>
			<v-app-bar-title><b>iHat</b></v-app-bar-title>
      <template v-slot:append>
        <v-btn icon="mdi-heart"></v-btn>
        <v-btn icon="mdi-magnify"></v-btn>
        <v-btn icon="mdi-dots-vertical"></v-btn>
      </template>
    </v-app-bar>
    <v-btn> Button </v-btn> 
  </v-app>
</template> 






