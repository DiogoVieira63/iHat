#!/bin/bash

# MongoDB connection information
MONGO_HOST="localhost"
MONGO_PORT="27017"

# Database and collection names
DB_NAME="iHatDB"
# Connect to MongoDB
mongosh --host ${MONGO_HOST}:${MONGO_PORT} <<EOF

use ${DB_NAME}

db.dropDatabase()
EOF
