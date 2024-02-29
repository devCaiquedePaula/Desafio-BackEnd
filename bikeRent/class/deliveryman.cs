using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Entregador
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    public int Identificador { get; set; }

    [BsonRequired]
    public string Nome { get; set; }

    [BsonRequired]
    [BsonUnique]
    public string CNPJ { get; set; }

    [BsonRequired]
    public DateTime DataNascimento { get; set; }

    [BsonRequired]
    [BsonUnique]
    public string NumeroCNH { get; set; }

    [BsonRequired]
    public string TipoCNH { get; set; }

    [BsonRequired]
    public string ImagemCNH { get; set; }

    [BsonRequired]
    public string MotoId { get; set; }

    [BsonRequired]
    public bool LocacaoAtiva { get; set; }

    [BsonRequired]
    public string ImagemCnhPath { get; set; }

    public bool LocacaoAtiva { get; set; }
    public DateTime DataInicioLocacao { get; set; }
    public PlanoLocacao PlanoLocacao { get; set; }
}
