<script setup lang="ts">
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'
import { object, string } from 'yup'
import FormMapa from './FormMapa.vue'

const dialog = ref(false)

type Estado = 'Planeada' | 'Pendente' | 'Em curso'
const estados: Estado[] = ['Planeada', 'Pendente', 'Em curso']

interface FormObra {
    nomeObra: string
    estado: Estado
}

const { handleSubmit, resetForm } = useForm<FormObra>({
    validationSchema: object({
        nomeObra: string().min(2).required(),
        estado: string().required()
    })
})

const nomeObra = useField('nomeObra')
const estado = useField<Estado>('estado')
const notSubmited = ref(true)

const submit = handleSubmit((values) => {
    notSubmited.value = false
    alert(JSON.stringify(values, null, 2))
    resetForm()
})

const close = () => {
    dialog.value = false
    notSubmited.value = true
    resetForm()
}
</script>

<template>
    <v-row justify="center">
        <v-dialog
            v-model="dialog"
            width="1024"
            persistent
        >
            <template v-slot:activator="{ props }">
                <v-btn
                    icon
                    variant="flat"
                    color="primary"
                    v-bind="props"
                >
                    <v-icon>mdi-plus</v-icon>
                </v-btn>
            </template>
            <v-card>
                <v-card-title>
                    <v-row class="mt-2">
                        <v-spacer></v-spacer>
                        <h1 class="text-h5">
                            {{ notSubmited ? 'Registar Obra' : 'Adicionar Mapa' }}
                        </h1>
                        <v-spacer></v-spacer>
                        <v-btn
                            icon
                            @click="close"
                            variant="text"
                        >
                            <v-icon>mdi-close</v-icon>
                        </v-btn>
                    </v-row>
                </v-card-title>
                <v-card-text>
                    <v-container>
                        <v-form
                            @submit.prevent="submit"
                            v-if="notSubmited"
                        >
                            <v-row>
                                <v-col
                                    cols="12"
                                    md="6"
                                >
                                    <v-text-field
                                        v-model="nomeObra.value.value"
                                        :error-messages="nomeObra.errorMessage.value"
                                        label="Nome da Obra*"
                                    ></v-text-field>
                                </v-col>

                                <v-col
                                    cols="12"
                                    md="6"
                                >
                                    <v-select
                                        v-model="estado.value.value"
                                        :error-messages="estado.errorMessage.value"
                                        :items="estados"
                                        label="Estado*"
                                    ></v-select>
                                </v-col>
                            </v-row>
                            <v-btn
                                color="primary"
                                type="submit"
                                block
                                class="mt-2"
                                >Submit</v-btn
                            >
                        </v-form>
                        <FormMapa v-else />
                    </v-container>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-row>
</template>
