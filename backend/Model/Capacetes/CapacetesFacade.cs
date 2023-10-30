    using iHat.Model.Obras;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    namespace iHat.Model.Capacetes;

    public class CapacetesFacade: ICapacetesFacade{

        public readonly IMongoCollection<Capacete> _capaceteCollection;
        public readonly IMongoCollection<Obra> _obraCollection;

        public CapacetesFacade(IOptions<DatabaseSettings> iHatDatabaseSettings){
            var mongoClient = new MongoClient(
                iHatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                iHatDatabaseSettings.Value.DatabaseName);

            _capaceteCollection = mongoDatabase.GetCollection<Capacete>(
                iHatDatabaseSettings.Value.CapacetesCollectionName);
        }
        // ... implement the methods here, e.g., GetAll(), Add() etc...

        public async Task<List<Capacete>> GetAll(){
            return await _capaceteCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Capacete> GetById(string id){
            return await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Capacete>> GetAllCapacetesdaObra(string idObra){
            //duvidas
            var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
            var lista = new List<Capacete>();
            foreach (var capacete in obra.Capacetes)
            {
                lista.Add(await _capaceteCollection.Find(x => x.Id == capacete).FirstOrDefaultAsync());
            }
            return lista;
        }

        public async Task Add(Capacete capacete){
            await _capaceteCollection.InsertOneAsync(capacete);
        }

        public async Task DeleteCapaceteToObra(string id){
            var capaceteQuery = _capaceteCollection.Find(x => x.Id == id);
            var capacete = await capaceteQuery.FirstOrDefaultAsync(); // Execute the query to retrieve the document
            if (capacete != null && capacete.Status != "Em uso")
            {
                await _capaceteCollection.DeleteOneAsync(x => x.Id == id);
            }
        }
        
        public async Task AddCapaceteToObra(string idCapacete, string idObra){
            // var capacete = await _capaceteCollection.Find(x => x.Id == idCapacete).FirstOrDefaultAsync();
            var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
            obra.Capacetes.Add(idCapacete);
        }   

    }
