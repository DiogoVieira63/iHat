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
    }
}