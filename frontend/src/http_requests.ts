import axios from 'axios'
import type { Obra, Capacete, CapacetePost, ObraPost } from './interfaces'
const url = `http://localhost:5069/ihat`

// Obras

export class ObraService {
    static getObras(): Promise<Obra[]> {
        return axios
            .get(`${url}/constructions`)
            .then((response) => {
                console.log(response.data)
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static getOneObra(id: string): Promise<Obra> {
        return axios
            .get(`${url}/constructions/${id}`)
            .then((response) => {
                console.log(response.data)
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

    static addOneObra(body: ObraPost): Promise<Obra> {
        return axios
            .post(`${url}/constructions`, body)
            .then((response) => {
                return response.data
            })
            .catch((error) => console.error('Error:', error))
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
                console.log(response.data)
                return response.data
            })
            .catch((error) => console.error('Error:', error))
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
        formData.append('file', file);
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
                console.log(response.data)
                return response.data
            })
            .catch((error) => console.error('Error:', error))
    }

    static addOneCapacete(body: CapacetePost): Promise<boolean> {
        return axios
            .post(`${url}/helmets`, body)
            .then((response) => {
                if (response.status === 200) {
                    // mudar para 201
                    return true
                } else return false
            })
            .catch((_) => {
                return false
            })
    }
}
