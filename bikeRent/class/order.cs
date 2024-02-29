using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    public int Identificador { get; set; }

    [BsonRequired]
    public DateTime DataCriacao { get; set; }

    [BsonRequired]
    public decimal ValorCorrida { get; set; }

    [BsonRequired]
    public string Situacao { get; set; }
}

public class RabbitMQConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string QueueName { get; set; }
}

public class MongoDBConfig
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public string PedidosCollection { get; set; }
}
