<script setup lang="ts">
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'

const dialog = ref(false)
// const estado = ref("Planeada");

// async function submit (event){
// 	const results = await event
// 	alert(`${estado.value} e ${nomeObra.value}`);
// };

const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    nomeObra(value) {
      if (value?.length >= 2) return true
      return message
    },
    estado(value) {
      if (value) return true
      return 'Selecione um dos items.'
    }
  }
})

const message = 'Campo obrigatório.'
const nomeObra = useField('nomeObra')
const estado = useField('estado')

const submit = handleSubmit((values) => {
  alert(JSON.stringify(values, null, 2))
})
</script>

<template>
  <v-row justify="center">
    <v-dialog v-model="dialog" width="1024">
      <template v-slot:activator="{ props }">
        <v-btn icon variant="flat" color="primary" v-bind="props">
          <v-icon>mdi-plus</v-icon>
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
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="nomeObra.value.value"
                    :error-messages="nomeObra.errorMessage.value"
                    label="Nome da Obra*"
                  ></v-text-field>
                </v-col>

                <v-col cols="12" md="6">
                  <v-select
                    v-model="estado.value.value"
                    :error-messages="estado.errorMessage.value"
                    :items="['Planeada', 'Pendente', 'Em curso']"
                    label="Estado*"
                  ></v-select>
                </v-col>
                <v-file-input
                  label="Selecione um arquivo do tipo IFC."
                  accept=".ifc"
                  density="compact"
                ></v-file-input>
              </v-row>
            </v-container>
            <v-alert color="info" icon="$info" text="* indicates required field"> </v-alert>
          </v-card-text>
          <v-btn type="submit" block class="mt-2">Submit</v-btn>
        </v-form>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<!-- 

<template>
  <form @submit.prevent="submit">
    <v-text-field
      v-model="name.value.value"
      :counter="10"
      :error-messages="name.errorMessage.value"
      label="Name"
    ></v-text-field>

    <v-text-field
      v-model="phone.value.value"
      :counter="7"
      :error-messages="phone.errorMessage.value"
      label="Phone Number"
    ></v-text-field>

    <v-text-field
      v-model="email.value.value"
      :error-messages="email.errorMessage.value"
      label="E-mail"
    ></v-text-field>

    <v-select
      v-model="select.value.value"
      :items="items"
      :error-messages="select.errorMessage.value"
      label="Select"
    ></v-select>

    <v-checkbox
      v-model="checkbox.value.value"
      :error-messages="checkbox.errorMessage.value"
      value="1"
      label="Option"
      type="checkbox"
    ></v-checkbox>

    <v-btn class="me-4" type="submit"> submit </v-btn>

    <v-btn @click="handleReset"> clear </v-btn>
  </form>
</template>

<script setup>
  import { ref } from 'vue'
  import { useField, useForm } from 'vee-validate'

  const { handleSubmit, handleReset } = useForm({
    validationSchema: {
      nome(value) {
        if (value?.length >= 2) return true

        return 'Name needs to be at least 2 characters.'
      },
      phone(value) {
        if (value?.length > 9 && /[0-9-]+/.test(value)) return true

        return 'Phone number needs to be at least 9 digits.'
      },
      email(value) {
        if (/^[a-z.-]+@[a-z.-]+\.[a-z]+$/i.test(value)) return true

        return 'Must be a valid e-mail.'
      },
      select(value) {
        if (value) return true

        return 'Select an item.'
      },
      checkbox(value) {
        if (value === '1') return true

        return 'Must be checked.'
      },
    },
  })
  const name = useField('name')
  const phone = useField('phone')
  const email = useField('email')
  const select = useField('select')
  const checkbox = useField('checkbox')

  const items = ref(['Item 1', 'Item 2', 'Item 3', 'Item 4'])

  const submit = handleSubmit(values => {
    alert(JSON.stringify(values, null, 2))
  })
</script> -->
