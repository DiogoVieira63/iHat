export interface Obra {
    id?: string,
    idResponsavel?: number,
    name?: string,
    zonas?: Array<string>, // temporario
    mapa?: string, // temporário
    capacetes?: Array<string>, //temporario
    status?: string
    mapas: Array<Mapa>
}

export interface Mapa {
    name: string
    svg: string
    zones: Array<Zone>
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
  nCapacete: number,
  status: string,
  info?: string,
  trabalhador?: string
}

export interface CapacetePost {
    nCapacete: number;
}

export interface ObraPost {
    name: string,
    mapa: string,
    status: string
}

export interface Header {
    name: string
    key: string
    params: Array<string>
}
