using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;

[ApiController]
[Route("api/entregadores")]
public class EntregadorController : ControllerBase
{
    private readonly IMongoCollection<Entregador> _entregadorCollection;

    public EntregadorController(IMongoDatabase database)
    {
        _entregadorCollection = database.GetCollection<Entregador>("entregadores");
    }

[HttpPost]
public IActionResult CadastrarEntregador([FromBody] Entregador novoEntregador)
{
    // Verificar se o CNPJ já está em uso
    var cnpjExistente = _entregadorCollection.Find(e => e.CNPJ == novoEntregador.CNPJ).Any();
    if (cnpjExistente)
    {
        return BadRequest("Já existe um entregador cadastrado com este CNPJ.");
    }

    // Verificar se o número da CNH já está em uso
    var cnhExistente = _entregadorCollection.Find(e => e.NumeroCNH == novoEntregador.NumeroCNH).Any();
    if (cnhExistente)
    {
        return BadRequest("Já existe um entregador cadastrado com este número de CNH.");
    }

    // Verificar se o TipoCNH é válido (A, B ou A+B)
    if (novoEntregador.TipoCNH != "A" && novoEntregador.TipoCNH != "B" && novoEntregador.TipoCNH != "A+B")
    {
        return BadRequest("Tipo de CNH inválido. Os tipos válidos são A, B ou A+B.");
    }

    // Adicionar lógica para verificar a existência da moto, validar outros campos, etc.
    _entregadorCollection.InsertOne(novoEntregador);
    return Ok("Entregador cadastrado com sucesso!");
}

}
