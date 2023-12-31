export interface Obra {
    id?: string,
    idResponsavel?: number,
    name: string,
    zonas?: Array<string>, // temporario
    capacetes?: Array<string>, //temporario
    status?: string
    mapa?: Array<Mapa>
}

export interface Mapa {
    id: string
    name: string
    svg: string
    zonas: Array<Zone>
}

export interface Zone {
    id: number
    points: Array<Point>
}

export interface Point {
    x: number
    y: number
}

export interface Capacete {
  id?: string 
  NCapacete: number,
  status: string,
  info?: string,
  trabalhador?: string
}

export interface Header {
    name: string
    key: string
    params: Array<string>
}
