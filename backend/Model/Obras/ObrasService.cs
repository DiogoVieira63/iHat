using iHat.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Obras;

public class ObrasService: IObrasService{

    public readonly IMongoCollection<Obra> _obraCollection;

    public ObrasService(IOptions<DatabaseSettings> iHatDatabaseSettings){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _obraCollection = mongoDatabase.GetCollection<Obra>(
            iHatDatabaseSettings.Value.BooksCollectionName);
    }


    /*

    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);
}
    */


    public async Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel){
        var obras = await _obraCollection.Find(x => x.IdResponsavel == idResponsavel).ToListAsync();

        if (obras.Count == 0)
        {
            // Nenhum resultado encontrado, retornar uma lista vazia ou fazer outra ação apropriada.
            return new List<Obra>();
        }
        return obras;
    }

    public async Task AddObra(string name, int idResponsavel){
        var newObra = new Obra(name, idResponsavel);

        var checkIfConstructionSameName = 
            await _obraCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        if(checkIfConstructionSameName != null){
            throw new Exception("Construction with this name already exists.");
        }

        try{
            await _obraCollection.InsertOneAsync(newObra);
        }
        catch (Exception e){
            throw new Exception(e.Message);
        }
    }
}