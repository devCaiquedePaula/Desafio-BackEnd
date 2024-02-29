using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class NotificacaoPedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string EntregadorId { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataNotificacao { get; set; }

    public string EntregadorId { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataNotificacao { get; set; }
}
