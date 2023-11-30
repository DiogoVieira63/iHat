export interface Obra {
    _id: string,
    IdResponsavel: number,
    Name: string,
    Zonas?: Array<string>, // temporario
    Mapa?: string, // temporário
    Capacetes?: Array<string>, //temporario
    Status: string
}
  
export interface Capacete {
  _id?: string 
  NCapacete: number,
  Status: string,
  Info: string,
  Trabalhador: string
}

export interface CapacetePost {
    NCapacete: number;
}

export interface ObraPost {
    Name: string,
    Mapa: string,
    Status: string
}

export interface Header {
    name: string,
    key: string,
    params: Array<string>
}