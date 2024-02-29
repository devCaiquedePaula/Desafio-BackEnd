using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Moto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    public int Identificador { get; set; }

    [BsonRequired]
    public int Ano { get; set; }

    [BsonRequired]
    public string Modelo { get; set; }

    [BsonRequired]
    [BsonUnique]
    public string Placa { get; set; }
}
