import axios from 'axios'
import type { Obra, Capacete } from './interfaces'
const url = `http://localhost:5069/ihat`

// Obras

export class ObraService {
    static getObras(): Promise<Obra[]> {
        return axios
            .get(`${url}/constructions`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static getOneObra(id: string): Promise<Obra> {
        return axios
            .get(`${url}/constructions/${id}`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static deleteOneObra(id: string): Promise<void> {
        return axios
            .delete(`${url}/constructions/${id}`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static addOneObra(body: Obra): Promise<string> {
        const formData = new FormData();
        formData.append('name', body.name);
        return axios
            .post(`${url}/constructions`, formData)
            .then((response) => {
                if (response.status === 200) {
                    return response.data
                }
                throw new Error('Error')
            })
            .catch((error) => console.error('Error:', error.response))
    }

    static addCapaceteToObra(idObra: string, idCapacete: string): Promise<void> {
        return axios
            .post(`${url}/constructions/${idObra}/helmets/${idCapacete}`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static getCapacetesFromObra(idObra: string): Promise<Capacete[]> {
        return axios
            .get(`${url}/constructions/${idObra}/helmets`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static updateNomeObra(idObra: string, name: string): Promise<void> {
     return axios.patch(`${url}/constructions/${idObra}`, name, {headers: {'Content-Type': 'application/json',}})
       .then((response) => {
           return response.data
       })
       .catch((error) => console.error('Error:', error));
    }

    static changeEstadoObra(idObra: string, state: string): Promise<void> {
     return axios.patch(`${url}/constructions/${idObra}/state`, state, {headers: {'Content-Type': 'application/json',}})
       .then((response) => {
           return response.data
       })
       .catch((error) => console.error('Error:', error));
    }

    static deleteCapaceteFromObra(idObra: string, idCapacete: string): Promise<void> {
        return axios
            .delete(`${url}/constructions/${idObra}/helmets/${idCapacete}`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static addMapaToObra(idObra: string, file : File): Promise<void> {
        const formData = new FormData();
        formData.append('Mapa', file);
        return axios
            .post(`${url}/constructions/${idObra}/map`, formData, {headers : {
                'Content-Type': 'multipart/form-data'
            }})
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }
}

export class CapaceteService {
    static getCapacetes(): Promise<Capacete[]> {
        return axios
            .get(`${url}/helmets`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static getOneCapacete(id: string): Promise<Capacete> {
        return axios
            .get(`${url}/helmets/${id}`)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static addOneCapacete(body: Capacete): Promise<void> {
        return axios
            .post(`${url}/helmets`, body)
            .then((response) => {
                if (response.status === 200) 
                    return response.data
                else throw new Error('Error')
            })
            .catch((_) => {
                console.error('Error in Axios')
            })
    }
}