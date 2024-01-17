#!/bin/bash

# MongoDB connection information
MONGO_HOST="localhost"
MONGO_PORT="27017"

# Database and collection names
DB_NAME="iHatDB"
OBRAS_COLLECTION="Obras"
CAPACETES_COLLECTION="Capacetes"
LOGS_COLLECTION="Logs"
MENSAGENS_CAPACETE_COLLECTION="MensagensCapacete"
MAPAS_COLLECTION="Mapas"

# Connect to MongoDB
mongosh --host ${MONGO_HOST}:${MONGO_PORT} <<EOF

use ${DB_NAME}

db.createCollection("${OBRAS_COLLECTION}")

db.${OBRAS_COLLECTION}.insertMany( [
    {
        "_id": ObjectId("000000000000000000000001"),
        "IdResponsavel":1,
        "Nome":"Primeira Obra",
        "Mapa": [],
        "Capacetes":[],
        "Status":"Pendente"
    },
    {
        "_id": ObjectId("000000000000000000000002"),
        "IdResponsavel":1,
        "Nome":"Segunda Obra",
        "Mapa": [],
        "Capacetes":[1,2,3,4],
        "Status":"Em Curso"
    }
] )


db.createCollection("${CAPACETES_COLLECTION}")

db.${CAPACETES_COLLECTION}.insertMany( [
    {
        "Numero":1,
        "Status":"Em Uso",
        "Trabalhador":"a01",
        "Obra":"000000000000000000000002"
    },
    {
        "Numero":2,
        "Status":"Livre",
        "Trabalhador": null,
        "Obra":"000000000000000000000002"
    },
    {
        "Numero":3,
        "Status":"Em Uso",
        "Trabalhador":"a02",
        "Obra":"000000000000000000000002"
    },
    {
        "Numero":4,
        "Status":"Em Uso",
        "Trabalhador":"a03",
        "Obra":"000000000000000000000002"
    },
    {
        "Numero":5,
        "Status":"Livre",
        "Trabalhador":null,
        "Obra":null
    },
    {
        "Numero":6,
        "Status":"Livre",
        "Trabalhador":null,
        "Obra":null

    },
    {
        "Numero":7,
        "Status":"Livre",
        "Trabalhador":null,
        "Obra":null
    },
    {
        "Numero":8,
        "Status":"Livre",
        "Trabalhador":null,
        "Obra":null
    }
  ]
)

db.createCollection("${LOGS_COLLECTION}")

db.createCollection("${MENSAGENS_CAPACETE_COLLECTION}")

db.${MENSAGENS_CAPACETE_COLLECTION}.insertMany( [
    {
        NCapacete: 1,
        Type: 'ValueUpdate',
        Fall: false,
        BodyTemperature: { Value: 36.1 },
        Heartrate: { Value: 100 },
        Proximity: '10',
        Position: '?',
        Location: { X: 0, Y: 0, Z: 0 },
        Gases: { Metano: 0, MonoxidoCarbono: 0 }
  }
] )

db.createCollection("${MAPAS_COLLECTION}")

EOF
