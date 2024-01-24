# iHat

## How to Run

Uma solução em Docker está a ser desenvolvida mas enquanto ainda não existe:

### Mqtt Brocker
```
cd mqtt_brocker
docker compose up
```

### Backend

Instalar Dotnet v7
```
cd backend
dotnet run
```

### Frontend

```
cd frontend
npm install
npm run serve
```

### Model2Svg

```
cd model2SVG
pip install -r requirements.txt
python3 api_ifc.py
```
### Base de dados

Instalar Mongo DB

Para a criação e povoamento inicial da base de dados:
```
./mongoDB.sh
```
Para remover a base de dados:
```
./delMongo.sh
```
