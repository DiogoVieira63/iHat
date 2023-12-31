<script setup lang="ts">
    import PageLayout from '@/components/Layouts/PageLayout.vue'

    const items = [
        {
          name: 'António Fernandes',
          role: `Backend`,
          img: '/avatar.png',
        },
        {
          name: 'Diogo Vieira',
          role: `Frontend`,
          img: '/avatar.png',
        },
        {
          name: 'Eduardo Magalhães',
          role: `Frontend`,
          img: '/avatar.png',
        },
        {
          name: 'João Mendes',
          role: `Backend`,
          img: '/avatar.png',
        },
        {
          name: 'João Torres',
          role: `Frontend`,
          img: '/avatar.png',
        },
        {
          name: 'Mariana Rodrigues',
          role: `Backend`,
          img: '/avatar.png',
        },
        {
          name: 'Rui Alves',
          role: `Backend`,
          img: '/avatar.png',
        },
      ];

    const transparent: string = 'rgba(255, 255, 255, 0)';

</script>

<style scoped>
    .v-card {
      transition: opacity .4s ease-in-out;
    }

    .v-card:not(.on-hover) {
      opacity: 0.6;
    }

    .second-row {
      display: flex;
      justify-content: center;
    }

    .second-row .d-flex {
      justify-content: space-between;
      flex-wrap: wrap;
    }
</style>

<template>
  <PageLayout>
    <v-row>
        <v-col align="center" justify="center" class="py-16">

        </v-col>
        <v-col align="center" justify="center">
            <v-container class="pa-4 text-center">
                <img src="/team.jpg" alt="iHat Team" />
            </v-container>
        </v-col>
        <v-col>

        </v-col>
    </v-row>
    <v-row v-for="index in 2" :key="index" class="py-4" :class="{ 'second-row': index === 2 }">
      <v-col
        v-for="(item, i) in items.slice((index - 1) * 4, ((index - 1) * 4)+4)"
        :key="i"
        cols="12"
        md="3"
        class="d-flex justify-center"
      >
        <v-hover v-slot="{ isHovering, props }">
          <v-card
            :elevation="isHovering ? 12 : 2"
            :class="{ 'on-hover': isHovering }"
            v-bind="props"
            height="350px"
            width="350px"
            align="center"
            justify="center"
          >
            <v-img
              :src="item.img"
              height="350px"
              width="350px"
              cover
            >
              <v-card-title class="text-h6 text-white d-flex flex-column">
                <p class="mt-4">{{ item.name }}</p>
                <div>
                  <p class="ma-0 text-body-1 font-weight-bold">{{ item.role }}</p>
                </div>
              </v-card-title>
            </v-img>
          </v-card>
        </v-hover>
      </v-col>
    </v-row>
  </PageLayout>
</template>
