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

ZONAS_RISCO_COLLECTION="ZonasRisco"
MAPAS_COLLECTION="Mapas"

# Connect to MongoDB
mongosh --host ${MONGO_HOST}:${MONGO_PORT} <<EOF

use ${DB_NAME}

db.createCollection("${OBRAS_COLLECTION}")

db.${OBRAS_COLLECTION}.insertMany( [
    {
        "IdResponsavel":1,
        "Name":"Primeira Obra",
        "Mapa": [],
        "Capacetes":[],
        "Status":"Pendente"
    },
    {
        "IdResponsavel":1,
        "Name":"Segunda Obra",
        "Mapa": [],
        "Capacetes":[1,2,3,4],
        "Status":"Em Curso"
    }
] )

db.createCollection("${CAPACETES_COLLECTION}")

db.${CAPACETES_COLLECTION}.insertMany( [
    {
        "NCapacete":1,
        "Status":"Em Uso",
        "Trabalhador":"a01",
    },
    {
        "NCapacete":2,
        "Status":"Associado à Obra",
        "Trabalhador": null,
    },
    {
        "NCapacete":3,
        "Status":"Em Uso",
        "Trabalhador":"a02",
    },
    {
        "NCapacete":4,
        "Status":"Em Uso",
        "Trabalhador":"a03"
    },
    {
        "NCapacete":5,
        "Status":"Livre",
        "Trabalhador":""
    },
    {
        "NCapacete":6,
        "Status":"Livre",
        "Trabalhador":""
    },
    {
        "NCapacete":7,
        "Status":"Livre",
        "Trabalhador":""
    },
    {
        "NCapacete":8,
        "Status":"Livre",
        "Trabalhador":""
    }
] )

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

db.createCollection("${ZONAS_RISCO_COLLECTION}")

db.createCollection("${MAPAS_COLLECTION}")

EOF
