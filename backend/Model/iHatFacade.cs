using System.Globalization;
using System.Runtime.CompilerServices;
using iHat.Model.Obras;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;

    public iHatFacade(IObrasService obrasService){
        iobras = obrasService;
    }

    public void NewConstruction(string name){
        // Guarda na Base
        iobras.AddObra(name);
    }


    public async Task<List<Obra>> GetObras(int idResponsavel){

        var obras = await iobras.GetObrasOfResponsavel(idResponsavel);

        if(obras == null){
            Console.WriteLine("[iHatFacade] Lista de obras vazia.");
        }

        return obras;
    }


    public async void AlteraEstadoObra(string id, string estado){

        iobras.AlteraEstadoObra(id, estado);

    }

}