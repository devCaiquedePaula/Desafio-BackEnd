using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;

[ApiController]
[Route("api/entregadores")]
public class EntregadorController : ControllerBase
{
    private readonly IMongoCollection<Entregador> _entregadorCollection;
    private readonly IMongoCollection<Moto> _motoCollection;
    private readonly IMongoCollection<Pedido> _pedidoCollection;
    private readonly IMongoCollection<NotificacaoPedido> _notificacaoCollection;

    public EntregadorController(IMongoDatabase database)
    {
        _entregadorCollection = database.GetCollection<Entregador>("entregadores");
        _motoCollection = database.GetCollection<Moto>("motos");
        _pedidoCollection = database.GetCollection<Pedido>("pedidos");
        _notificacaoCollection = database.GetCollection<NotificacaoPedido>("notificacoesPedidos");
    }

    [HttpPut("{id}/devolver")]
    public IActionResult DevolverMoto(string id, [FromBody] DateTime dataDevolucao)
    {
        // Verificar se o entregador existe
        var entregador = _entregadorCollection.Find(e => e.Id == id).FirstOrDefault();
        if (entregador == null)
        {
            return NotFound("Entregador não encontrado.");
        }

        // Verificar se o entregador tem uma locação ativa
        if (!entregador.LocacaoAtiva)
        {
            return BadRequest("Não há locação ativa para este entregador.");
        }

        // Encontrar a moto associada ao entregador
        var motoLocada = _motoCollection.Find(m => m.Id == entregador.MotoId).FirstOrDefault();
        if (motoLocada == null)
        {
            return NotFound("Moto não encontrada.");
        }

        // Calcular o valor total da locação com base na data de devolução
        var diasLocacao = (int)(dataDevolucao - entregador.DataInicioLocacao).TotalDays;

        double custoDiario;
        switch (entregador.PlanoLocacao)
        {
            case PlanoLocacao.SeteDias:
                custoDiario = 30.00;
                break;
            case PlanoLocacao.QuinzeDias:
                custoDiario = 28.00;
                break;
            case PlanoLocacao.TrintaDias:
                custoDiario = 22.00;
                break;
            default:
                return BadRequest("Plano de locação inválido.");
        }

        var custoTotal = custoDiario * diasLocacao;

        // Aplicar penalidades por devolução fora do prazo
        if (dataDevolucao < entregador.DataPrevisaoTermino)
        {
            var diasAtraso = (int)(entregador.DataPrevisaoTermino - dataDevolucao).TotalDays;

            double multaPercentual;
            switch (entregador.PlanoLocacao)
            {
                case PlanoLocacao.SeteDias:
                    multaPercentual = 0.20;
                    break;
                case PlanoLocacao.QuinzeDias:
                    multaPercentual = 0.40;
                    break;
                case PlanoLocacao.TrintaDias:
                    multaPercentual = 0.60;
                    break;
                default:
                    return BadRequest("Plano de locação inválido.");
            }

            var multa = custoDiario * diasAtraso * multaPercentual;
            custoTotal += multa;
        }
        else if (dataDevolucao > entregador.DataPrevisaoTermino)
        {
            var diasAdicionais = (int)(dataDevolucao - entregador.DataPrevisaoTermino).TotalDays;
            var custoAdicional = diasAdicionais * 50.00;
            custoTotal += custoAdicional;
        }

        // Atualizar o status da moto e limpar os dados de locação no entregador
        motoLocada.LocacaoAtiva = false;
        _motoCollection.ReplaceOne(m => m.Id == motoLocada.Id, motoLocada);

        entregador.LocacaoAtiva = false;
        entregador.DataInicioLocacao = default(DateTime);
        entregador.DataPrevisaoTermino = default(DateTime);
        entregador.PlanoLocacao = PlanoLocacao.SeteDias; // Ou outro valor padrão
        _entregadorCollection.ReplaceOne(e => e.Id == entregador.Id, entregador);

        return Ok($"Moto devolvida com sucesso. Custo total da locação: R${custoTotal:F2}");
    }

    [HttpPost("{id}/aceitar-pedido/{pedidoId}")]
    public IActionResult AceitarPedido(string id, string pedidoId)
    {
        // Verificar se o entregador existe
        var entregador = _entregadorCollection.Find(e => e.Id == id).FirstOrDefault();
        if (entregador == null)
        {
            return NotFound("Entregador não encontrado.");
        }

        // Verificar se o entregador tem uma locação ativa
        if (!entregador.LocacaoAtiva)
        {
            return BadRequest("Não há locação ativa para este entregador.");
        }

        // Verificar se o entregador já tem um pedido aceito
        if (!string.IsNullOrEmpty(entregador.PedidoAceitoId))
        {
            return BadRequest("O entregador já aceitou um pedido.");
        }

        // Verificar se o pedido existe
        var pedido = _pedidoCollection.Find(p => p.Id == pedidoId && p.Situacao == "Disponivel").FirstOrDefault();
        if (pedido == null)
        {
            return NotFound("Pedido não encontrado ou já foi aceito.");
        }

        // Verificar se o entregador foi notificado para este pedido
        var notificacao = _notificacaoCollection.Find(n => n.EntregadorId == id && n.Mensagem.Contains(pedidoId)).FirstOrDefault();
        if (notificacao == null)
        {
            return BadRequest("O entregador não foi notificado para este pedido.");
        }

        // Atualizar o estado do pedido para "Aceito"
        pedido.Situacao = "Aceito";
        pedido.EntregadorId = entregador.Id;

        _pedidoCollection.ReplaceOne(p => p.Id == pedidoId, pedido);

        // Atualizar o estado do entregador para indicar que ele aceitou um pedido
        entregador.PedidoAceitoId = pedidoId;
        _entregadorCollection.ReplaceOne(e => e.Id == entregador.Id, entregador);

        return Ok($"Pedido {pedidoId} aceito pelo entregador {entregador.Id}.");
    }
}
