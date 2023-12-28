using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace iHat.Model.Zonas
{
    public class ZonasRisco
    {
        [BsonId] // Primary key
        [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
        public string? Id { get; set; }
        public int IdZona { get; set; }
        public List<Tuple<double, double>>? Zonas { get; set; } //GeoJSON 

        public ZonasRisco(int idZona)
        {
            IdZona = idZona;
            Zonas = new List<Tuple<double, double>>();
        }

        /* algoritmo de Ray Casting
            Conta o número de interseções entre um raio horizontal que parte do ponto e as arestas do polígono. 
            Se o número de interseções for ímpar, o ponto está dentro do polígono; caso contrário, está fora.
        */
        public bool InsideZonaRisco(double x, double y){
            int count = 0;
            int n = Zonas!.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((Zonas[i].Item2 <= y && y < Zonas[j].Item2) || (Zonas[j].Item2 <= y && y < Zonas[i].Item2)) &&
                    (x < (Zonas[j].Item1 - Zonas[i].Item1) * (y - Zonas[i].Item2) / (Zonas[j].Item2 - Zonas[i].Item2) + Zonas[i].Item1))
                {
                    count++;
                }
            }
            return count % 2 == 1;
        }
    }    
}