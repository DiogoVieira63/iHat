<script setup>
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'

const { handleSubmit, handleReset } = useForm({
    validationSchema: {
      id(value) {
        if (value?.length >= 2) return true

        return 'Id needs to be at least 2 characters.'
      },
      estado(value) {
        if(value) return true

        return 'Select an item.'
      },
    },
  })

  const id = useField('id')
  const estado = useField('estado')
  const dialogCapacete = ref(false);
  // const items = ref(['1', '2', '3'])

  const submit = handleSubmit(values => {
    alert(JSON.stringify(values, null, 2))
  })




// const estadoCapacete = ref("1");
// const idCapacete = ref("");

// async function submit (event){
// 	const results = await event
// 	alert(`${estadoCapacete.value} e ${idCapacete.value}`);
// };

</script>


<template>
    <v-row justify="center">
      <v-dialog
        v-model="dialogCapacete"
        width="1024"
      >
        <template v-slot:activator="{ props }">
          <v-btn icon variant="flat" color="primary" v-bind="props">
            <v-icon  >mdi-plus</v-icon>
          </v-btn>
        </template>
        <v-card>
			<v-form @submit.prevent="submit">
          <v-card-title>
            <span class="text-h5">Registar Capacete</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
					          v-model="id.value.value"
                    label="id do Capacete*"
                    :error-messages="id.errorMessage.value"
                  ></v-text-field>
                </v-col>

                <v-col
                  cols="12"
                  md="6"
                >
                <v-select
                    v-model="estado.value.value"
                    :items="['1', '2', '3']"
                    label="Estado*"
                    :error-messages="estado.errorMessage.value"
                  ></v-select>
                </v-col>
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

