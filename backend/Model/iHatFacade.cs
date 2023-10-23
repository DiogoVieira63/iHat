using iHat.Model.Obras;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private IObrasService iobras;

    public void NewConstruction(string name){
        // Guarda na Base
        iobras.AddObra(name);
    }


}