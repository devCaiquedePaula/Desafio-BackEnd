using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IMongoCollection<NotificacaoPedido> _notificacaoCollection;
    private readonly IMongoCollection<Moto> _motoCollection;

    public AdminController(IMongoDatabase database)
    {
    _notificacaoCollection = database.GetCollection<NotificacaoPedido>("notificacoesPedidos");
    }

    [HttpPost("cadastrar-moto")]
    public IActionResult CadastrarMoto([FromBody] Moto moto)
    {
        _motoCollection.InsertOne(moto);
        return Ok($"Moto cadastrada com ID: {moto.Id}");
    }

    [HttpGet("consultar-motos")]
    public IActionResult ConsultarMotos()
    {
        var motos = _motoCollection.Find(Builders<Moto>.Filter.Empty).ToList();
        return Ok(motos);
    }
    [HttpGet("entregadores-notificados/{pedidoId}")]
    public IActionResult ObterEntregadoresNotificados(string pedidoId)
    {
        var notificacoes = _notificacaoCollection.Find(n => n.Mensagem.Contains(pedidoId)).ToList();
        var entregadoresIds = notificacoes.Select(n => n.EntregadorId).Distinct().ToList();

        // Lógica para obter detalhes dos entregadores a partir dos IDs 
        return Ok(entregadoresIds);
    }
}
