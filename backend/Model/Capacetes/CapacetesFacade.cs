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
                iHatDatabaseSettings.Value.BooksCollectionName);
        }
        // ... implement the methods here, e.g., GetAll(), Add() etc...

        public async Task<List<Capacete>> GetAll(){
            return await _capaceteCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Capacete> GetById(int id){
            return await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(Capacete capacete){
            await _capaceteCollection.InsertOneAsync(capacete);
        }

        public async Task Delete(int id)
        {
            await _capaceteCollection.DeleteOneAsync(x => x.Id == id);
        }

        
        //duvidas
        public async Task AddCapaceteToObra(int idCapacete, string idObra){
            // var capacete = await _capaceteCollection.Find(x => x.Id == idCapacete).FirstOrDefaultAsync();
            var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
            obra.Capacetes.Add(idCapacete);
        }   

    }
