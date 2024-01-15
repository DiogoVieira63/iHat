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
    floor: number
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

export interface ValueObject {
    value: number;
}

export interface Gases {
    metano: number;
    monoxidoCarbono: number;
}

export interface MensagemCapacete {
    id?: string 
    timestamp: Date, 
    nCapacete: number,
    type: string,
    fall: boolean,
    bodyTemperature: ValueObject,
    heartrate: ValueObject,
    proximity: number,
    position: Position,
    location: Position,
    gases: Gases,
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
