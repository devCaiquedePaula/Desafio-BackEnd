using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;

public class PedidoService
{
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;

    public PedidoService(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.QueueDeclare(queue: _configuration["RabbitMQ:QueueName"], durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublicarPedido(Pedido pedido)
    {
        var jsonPedido = JsonConvert.SerializeObject(pedido);
        var body = Encoding.UTF8.GetBytes(jsonPedido);

        _channel.BasicPublish(exchange: "", routingKey: _configuration["RabbitMQ:QueueName"], basicProperties: null, body: body);
    }

    var pedidoService = serviceProvider.GetRequiredService<PedidoService>();

    var novoPedido = new Pedido
    {
        Identificador = 1,
            DataCriacao = DateTime.Now,
        ValorCorrida = 50.00m,
        Situacao = "Disponivel"
    };
    pedidoService.PublicarPedido(novoPedido);
}
