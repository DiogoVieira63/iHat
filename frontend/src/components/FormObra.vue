<script setup>
import { ref } from 'vue'

const dialog = ref(false);
const estado = ref("Planeada");
const nomeObra = ref("");

async function submit (event){
	const results = await event
	alert(`${estado.value} e ${nomeObra.value}`);
};

</script>


<template>
    <v-row justify="center">
      <v-dialog
        v-model="dialog"
        width="1024"
      >
        <template v-slot:activator="{ props }">
          <v-btn icon color="primary" v-bind="props">
            <v-icon color="black">mdi-plus</v-icon>
          </v-btn>
        </template>
        <v-card>
					<v-form @submit.prevent="submit">
          <v-card-title>
            <span class="text-h5">Registar Obra</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
										v-model="nomeObra"
                    label="Nome da Obra*"
                    required
                  ></v-text-field>
                </v-col>

                <v-col
                  cols="12"
                  md="6"
                >
                <v-select
                    v-model="estado"
                    :items="['Planeada', 'Pendente', 'Em curso']"
                    label="Estado*"
                    required
                  ></v-select>
                </v-col>
                <v-file-input
                  label="File input"
                  density="compact"
                ></v-file-input>
              </v-row>
            </v-container>
            <v-alert
              color="info"
              icon="$info"
              text="* indicates required field"
            >
            </v-alert>
          	</v-card-text>
			<v-btn type="submit" block class="mt-2">Submit</v-btn>
			</v-form>
        </v-card>
      </v-dialog>
    </v-row>
  </template>