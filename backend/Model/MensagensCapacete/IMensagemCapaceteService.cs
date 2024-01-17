using iHat.MensagensCapacete.Values;

namespace iHat.Model.MensagensCapacete;
public interface IMensagemCapaceteService{

    /*
    Função que permite adicionar uma MensagemCapacete
    */
    Task Add(MensagemCapacete mensagem);

    /*
    Função que permite obter as últimas 20 mensagens recebidas do Capacete {nCapacete}
    Returns: Uma lista de MensagensCapacete
    */
    Task<List<MensagemCapacete>> GetUltimosDadosDoCapacete(int nCapacete);

    /*
    Função que permite obter a última localização do {nCapacete}
    Returns: A Location ou null
    */
    Task<Location?> GetLastLocation(int nCapacete);
}