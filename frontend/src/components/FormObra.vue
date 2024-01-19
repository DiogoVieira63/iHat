<script setup lang="ts">
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'
import { object, string } from 'yup'
import { ObraService } from '@/services/http'
import FormMapa from '@/components/FormMapa.vue'

const dialog = ref(false)

interface FormObra {
    nomeObra: string
}

const { handleSubmit, resetForm } = useForm<FormObra>({
    validationSchema: object({
        nomeObra: string().min(2).required()
    })
})

const nomeObra = useField('nomeObra')
const notSubmited = ref(true)
const idObra = ref('')

const emit = defineEmits(['update'])

const submit = handleSubmit((values, actions) => {
    const obra = {
        nome: values.nomeObra
    }
    ObraService.addOneObra(obra)
        .then((res_idObra) => {
            idObra.value = res_idObra
            notSubmited.value = false
            emit('update')
            resetForm()
        })
        .catch((error) => {
            actions.setFieldError('nomeObra', 'Erro na criação da Obra.')
            console.log(error)
        })
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
            <v-card rounded="xl">
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
                            <v-text-field
                                v-model="nomeObra.value.value"
                                :error-messages="nomeObra.errorMessage.value"
                                label="Nome da Obra*"
                            ></v-text-field>
                            <v-btn
                                color="primary"
                                type="submit"
                                block
                                class="mt-2"
                                rounded="xl"
                                >Submit</v-btn
                            >
                        </v-form>
                        <FormMapa
                            v-else
                            :id-obra="idObra"
                            :canCancel="false"
                            @update="close"
                        />
                    </v-container>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-row>
</template>
