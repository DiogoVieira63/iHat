using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text;

namespace iHat.Model.Zonas
{
    public class ZonasRisco
    {
        [BsonId] // Primary key
        [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
        public string? Id { get; set; }
        // public int Id { get; set; }
        public List<Point>? Points { get; set; } //GeoJSON 

        public ZonasRisco()
        {
            Points = new List<Point>();
        }

        public ZonasRisco(string idZona)
        {
            Id = idZona;
            Points = new List<Point>();
        }

    //     public override string ToString()
    // {
    //     StringBuilder sb = new StringBuilder();
    //     sb.AppendLine($"Id: {Id}");
    //     sb.AppendLine($"IdZona: {IdZona}");

    //     if (Zonas != null)
    //     {
    //         sb.AppendLine("Zonas:");
    //         foreach (var ponto in Zonas)
    //         {
    //             sb.AppendLine($"  Latitude: {ponto.X}, Longitude: {ponto.Y}");
    //         }
    //     }

    //     return sb.ToString();
    // }

        /* algoritmo de Ray Casting
            Conta o número de interseções entre um raio horizontal que parte do ponto e as arestas do polígono. 
            Se o número de interseções for ímpar, o ponto está dentro do polígono; caso contrário, está fora.
        */
        public bool InsideZonaRisco(double x, double y){
            int count = 0;
            int n = Points!.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((Points[i].Y <= y) && (y < Points[j].Y)) || ((Points[j].Y <= y) && (y < Points[i].Y)))
                {
                    if (x > (Points[j].X - Points[i].X) * (y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X)
                    {
                        count++;
                    }
                }
            }
            return count % 2 == 1;
        }
    }    
}