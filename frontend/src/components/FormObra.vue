<script setup lang="ts">
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'
import { object, string } from 'yup'

const dialog = ref(false)

type Estado = 'Planeada' | 'Pendente' | 'Em curso'
const estados: Estado[] = ['Planeada', 'Pendente', 'Em curso']

interface Form {
    nomeObra: string
    estado: Estado
}

const { handleSubmit } = useForm<Form>({
    validationSchema: object({
        nomeObra: string().min(2).required(),
        estado: string().required()
    })
})

const nomeObra = useField('nomeObra')
const estado = useField<Estado>('estado')

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
                                        v-model="nomeObra.value"
                                        :error-messages="nomeObra.errorMessage.value"
                                        label="Nome da Obra*"
                                    ></v-text-field>
                                </v-col>

                                <v-col cols="12" md="6">
                                    <v-select
                                        v-model="estado.value.value"
                                        :error-messages="estado.errorMessage.value"
                                        :items="estados"
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
                        <v-alert color="info" icon="$info" text="* indicates required field">
                        </v-alert>
                    </v-card-text>
                    <v-btn type="submit" block class="mt-2">Submit</v-btn>
                </v-form>
            </v-card>
        </v-dialog>
    </v-row>
</template>
