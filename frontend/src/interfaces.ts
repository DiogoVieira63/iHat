export interface Obra {
    id?: string
    idResponsavel?: number
    name: string
    zonas?: Array<string> // temporario
    capacetes?: Array<string> //temporario
    status?: string
    mapa?: Array<Mapa>
}

export interface Mapa {
    id: string
    name: string
    svg: string
    Floor: number
    zonas: Array<Zone>
}

export interface Zone {
    id: string
    points: Array<Point>
}

export interface Point {
    x: number
    y: number
}

export interface Capacete {
    id?: string
    nCapacete: number
    status: string
    info?: string
    trabalhador?: string
    position?: Position
}

export interface Position {
    x: number
    y: number
    z: number
}

export interface Header {
    name: string
    key: string
    params: Array<string>
}

export interface Log {
    id?: string
    type: string
    timestamp: Date
    idObra: string
    idCapacete?: number
    idTrabalhador?: string
    mensagem: string
}
