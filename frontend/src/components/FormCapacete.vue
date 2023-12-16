<script setup lang="ts">
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'
import { object, string } from 'yup'
import type { Capacete } from '@/interfaces';
import { CapaceteService } from '@/http_requests';
interface Form {
    id: string
}

const { handleSubmit } = useForm<Form>({
    validationSchema: object({
        id: string().min(2).required()
    })
})


const id = useField<string>('id')
const dialogCapacete = ref(false)

const emit = defineEmits(['update'])

const submit = handleSubmit(async (values, actions) => {
    const Capacete : Capacete = {
        NCapacete: Number(values.id),
        Status: "Disponivel",
        Info: "",
        Trabalhador: ""
    }

    CapaceteService.addOneCapacete(Capacete).then((success) => {
        console.log("Success",success)
        if (success){
            dialogCapacete.value = false
            id.value.value = ""
            id.errorMessage.value = ""
            emit('update')
        }
        else{
            actions.setFieldError('id', 'Capacete já existe');
        }
    }).catch((error) => {
        console.log(error)
    })

})
</script>

<template>
    <v-row justify="center">
        <v-dialog v-model="dialogCapacete" width="1024">
            <template v-slot:activator="{ props }">
                <v-btn icon variant="flat" color="primary" v-bind="props">
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
                            <v-row>
                                <v-col cols="12" md="6">
                                    <v-text-field
                                        v-model="id.value.value"
                                        label="id do Capacete*"
                                        :error-messages="id.errorMessage.value"
                                    ></v-text-field>
                                </v-col>
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
