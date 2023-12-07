#!/bin/bash

# MongoDB connection information
MONGO_HOST="localhost"
MONGO_PORT="27017"

# Database and collection names
DB_NAME="iHatDB"
OBRAS_COLLECTION="Obras"
CAPACETES_COLLECTION="Capacetes"
LOGS_COLLECTION="Logs"

# Connect to MongoDB
mongosh --host ${MONGO_HOST}:${MONGO_PORT} <<EOF

use ${DB_NAME}

db.createCollection("${OBRAS_COLLECTION}")

db.Obras.insertMany( [
    {
        "IdResponsavel":1,
        "Name":"Primeira Obra",
        "Zonas":[],
        "Mapa": "file1",
        "Capacetes":[],
        "Status":"Pendente"
    },
    {
        "IdResponsavel":1,
        "Name":"Segunda Obra",
        "Zonas":[],
        "Mapa": "file3",
        "Capacetes":[1,2],
        "Status":"Em Curso"
    }
] )

db.createCollection("${CAPACETES_COLLECTION}")

db.Capacetes.insertMany( [
    {
        "NCapacete":1,
        "Status":"Em Uso",
        "Trabalhador":"a01",
    },
    {
        "NCapacete":2,
        "Status":"Livre",
        "Trabalhador": null,
    }
] )

db.createCollection("${LOGS_COLLECTION}")

EOF