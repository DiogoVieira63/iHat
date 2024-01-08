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
        public List<Point>? Zonas { get; set; } //GeoJSON 

        public ZonasRisco(int idZona)
        {
            IdZona = idZona;
            Zonas = new List<Point>();
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
                if (((Zonas[i].Y <= y) && (y < Zonas[j].Y)) || ((Zonas[j].Y <= y) && (y < Zonas[i].Y)))
                {
                    if (x > (Zonas[j].X - Zonas[i].X) * (y - Zonas[i].Y) / (Zonas[j].Y - Zonas[i].Y) + Zonas[i].X)
                    {
                        count++;
                    }
                }
            }
            return count % 2 == 1;
        }
    }    
}