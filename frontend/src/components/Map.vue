<template>
    <div class="center">
      <div>
        <v-img
          class="bg-white"
          width="500"
          :aspect-ratio="1"
          :src="getCurrentImage"
          cover
        ></v-img>
      </div>
      <div>
        <v-row align="center"> 
            <v-col align="center">
                <v-pagination
                  v-model="page"
                  :length="imageUrls.length"
                  :total-visible="5"
                ></v-pagination>
            </v-col>
            <v-col align="center">
                <v-btn prepend-icon="$edit" variant="tonal" color="primary">
                    Editar zonas de risco
                </v-btn>
            </v-col>
        </v-row>
      </div>    
    </div>
</template>
  

<script>
  export default {
    data() {
      return {
        page: 1,
        imageUrls: [
          '/Duplex-0.svg',
          '/Duplex-1.svg',
          '/Duplex-2.svg',
          '/Duplex-3.svg',
        ],
      };
    },
    computed: {
      getCurrentImage() {
        const currentIndex = this.page - 1;
        const validIndex = Math.min(Math.max(currentIndex, 0), this.imageUrls.length - 1);  
        return this.imageUrls[validIndex];
      },
    },
  };
</script>

<style>
  .center {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 70vh; /* Optional: Center content vertically within the viewport */
  } 
</style>
  