public class PedidoLocacao
{
    public PlanoLocacao Plano { get; set; }
    public int DiasLocacao { get; set; }
    public int DiasPrevisaoTermino { get; set; }
}

public enum PlanoLocacao
{
    SeteDias,
    QuinzeDias,
    TrintaDias
}
