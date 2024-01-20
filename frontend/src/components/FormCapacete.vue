<script setup lang="ts">
import type { Capacete } from '@/interfaces'
import { CapaceteService } from '@/services/http'
import { useField, useForm } from 'vee-validate'
import { ref } from 'vue'
import { object, string } from 'yup'

interface Form {
    id: string
}

const { handleSubmit } = useForm<Form>({
    validationSchema: object({
        id: string().min(1).required()
    })
})

const id = useField<string>('id')
const dialogCapacete = ref(false)

const emit = defineEmits(['update'])

const submit = handleSubmit((values, actions) => {
    const Capacete: Capacete = {
        numero: Number(values.id),
        status: 'Disponivel',
        info: '',
        trabalhador: ''
    }

    CapaceteService.addOneCapacete(Capacete)
        .then(() => {
            dialogCapacete.value = false
            id.value.value = ''
            id.errorMessage.value = ''
            emit('update')
        })
        .catch((error) => {
            actions.setFieldError('id', 'Capacete já existe')
            console.log(error)
        })
})
</script>

<template>
    <v-row justify="center">
        <v-dialog
            v-model="dialogCapacete"
            width="1024"
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
                <v-form @submit.prevent="submit">
                    <v-card-title>
                        <span class="text-h5">Registar Capacete</span>
                    </v-card-title>
                    <v-card-text>
                        <v-container>
                            <v-text-field
                                v-model="id.value.value"
                                label="id do Capacete*"
                                :error-messages="id.errorMessage.value"
                            ></v-text-field>
                        </v-container>
                        <v-btn
                            type="submit"
                            color="primary"
                            block
                            >Submit</v-btn
                        >
                    </v-card-text>
                </v-form>
            </v-card>
        </v-dialog>
    </v-row>
</template>
