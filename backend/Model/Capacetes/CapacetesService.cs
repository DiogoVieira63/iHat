    using iHat.Model.Obras;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    namespace iHat.Model.Capacetes;

    public class CapacetesService: ICapacetesService{

        public readonly IMongoCollection<Capacete> _capaceteCollection;
        public readonly IMongoCollection<Obra> _obraCollection;

        public CapacetesService(IOptions<DatabaseSettings> iHatDatabaseSettings){
            var mongoClient = new MongoClient(
                iHatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                iHatDatabaseSettings.Value.DatabaseName);

            _capaceteCollection = mongoDatabase.GetCollection<Capacete>(
                iHatDatabaseSettings.Value.CapacetesCollectionName);

            _obraCollection = mongoDatabase.GetCollection<Obra>(
                iHatDatabaseSettings.Value.ObrasCollectionName);

        }

        // ... implement the methods here, e.g., GetAll(), Add() etc...

        public async Task<List<Capacete>> GetAll(){
            return await _capaceteCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Capacete> GetById(string id){
            return await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

//mudar: dividir em duas, buscar a obra pelo id e buscar a lista de capacetes dessa obra
        public async Task<List<Capacete>> GetAllCapacetesdaObra(string idObra){
            //duvidas
            var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
            var lista = new List<Capacete?>();
            foreach(var capacete in obra.Capacetes){
                lista.Add(await _capaceteCollection.Find(x => x.Id == capacete).FirstOrDefaultAsync());
            }
            return lista;            
        }

        public async Task Add(int nCapacete){
            //verificar se na _capaceteCollection nao existe nenhum capacete com esse nCapacete
            var c = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
            if(c == null){
                var capacete = new Capacete(nCapacete,"Livre", "", "");
                await _capaceteCollection.InsertOneAsync(capacete);
            }
            else{
                throw new Exception("Capacete já existe na base de dados.");
            }
        }

        public async Task DeleteCapaceteToObra(string id, string idObra){
            var capacete = await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (capacete != null)
            {
                if (capacete.Status == "Associado à obra")
                {
                    var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
                    if (obra != null)
                    {
                        obra.Capacetes.Remove(id);

                        // Atualize a coleção de obras
                        var obraFilter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
                        var obraUpdate = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
                        await _obraCollection.UpdateOneAsync(obraFilter, obraUpdate);

                        // Atualize a coleção de capacetes
                        // var capaceteFilter = Builders<Capacete>.Filter.Eq(x => x.Id, id);
                        // var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Obra, null);
                        // await _capaceteCollection.UpdateOneAsync(capaceteFilter, capaceteUpdate);
                        var capaceteFilter = Builders<Capacete>.Filter.Eq(x => x.Id, id);
                        await _capaceteCollection.DeleteOneAsync(capaceteFilter);
                    }
                    else
                    {
                        throw new Exception("Obra não encontrada.");
                    }
                }
                else
                {
                    throw new Exception("Capacete não pode ser removido da obra, pois não está em uso.");
                }
            }
            else
            {
                throw new Exception("Capacete não encontrado.");
            }
        }

        public async Task AddCapaceteToObra(string idCapacete, string idObra){
            // var capacete = await _capaceteCollection.Find(x => x.Id == idCapacete).FirstOrDefaultAsync();
            var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();

            if (obra != null)
            {
                obra.Capacetes.Add(idCapacete);
                var filter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
                var update = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);

                await _obraCollection.UpdateOneAsync(filter, update);
            }
        }
   

    }
