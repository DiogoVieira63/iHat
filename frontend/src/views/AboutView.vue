<script setup lang="ts">
    import PageLayout from '@/components/Layouts/PageLayout.vue';

    const items = [
        {
          name: 'António Fernandes',
          role: `Backend`,
          img: '/to.png',
        },
        {
          name: 'Diogo Vieira',
          role: `Frontend`,
          img: '/Diogo.png',
        },
        {
          name: 'Eduardo Magalhães',
          role: `Frontend`,
          img: '/edu_.jpg',
        },
        {
          name: 'João Mendes',
          role: `Backend`,
          img: '/joaoMendes.png',
        },
        {
          name: 'João Torres',
          role: `Frontend`,
          img: '/sinel.png',
        },
        {
          name: 'Mariana Rodrigues',
          role: `Backend`,
          img: '/Mariana.jpeg',
        },
        {
          name: 'Rui Alves',
          role: `Backend`,
          img: '/rui.jpg',
        },
      ];


</script>

<style scoped>
    .card-opacity {
      transition: opacity .4s ease-in-out;
    }

    .card-opacity:not(.on-hover) {
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
        <v-col align="center" justify="center">
            <v-container class="pa-4 text-center">
                <img src="/team.jpg" alt="iHat Team" />
            </v-container>
        </v-col>
        <v-col class="pa-8">
        <v-card
            class="mx-auto"
            elevation="5"
            height="535px"
          >       
            <v-card-title class="text-h4"><b>Quem somos?</b></v-card-title>
   
            <v-card-text class="text-h6 py-2">
              "Somos estudantes do 2º ano do mestrado em engenharia informática na Universidade do Minho. 
              <br>
              <br>
              No âmbito do desenvolvimento do projeto de informática colaboramos com a empresa DTX, especializada em soluções tecnológicas, com o objetivo de tornar a atividade da contrução civil mais eficiente e segura, utilizando a tecnologia à nossa disposição para proteger a vida humana e tentar melhorar as condições de trabalho.
              <br>
              <br>
              Assim, desenvolvemos esta aplicação  capaz de receber dados de capacetes equipados com sensores, que permitem a identificação de potenciais situações de risco que possam colocar em risco a segurança dos trabalhadores."
            </v-card-text>
          
            <v-card-actions>
              <v-list-item class="w-100">
                <template v-slot:prepend>
                  <v-avatar
                    icon="mdi-account-group"
                    color="primary"
                  ></v-avatar>
                </template>
              
                <v-list-item-title>A equipa de trabalho</v-list-item-title>
              
                <v-list-item-subtitle>iHat</v-list-item-subtitle>
              </v-list-item>
            </v-card-actions>
            <v-img
              cover
              height="250"
              src="/obra.jpg"
            ></v-img>
          </v-card>
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
            class="card-opacity"
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
