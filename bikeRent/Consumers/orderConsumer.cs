using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class PedidoConsumer
{
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<Pedido> _pedidoCollection;
    private readonly IMongoCollection<NotificacaoPedido> _notificacaoCollection;

    public PedidoConsumer(IConfiguration configuration, IMongoDatabase database)
    {
        _configuration = configuration;
        _pedidoCollection = database.GetCollection<Pedido>(_configuration["MongoDB:PedidosCollection"]);
        _notificacaoCollection = database.GetCollection<NotificacaoPedido>(_configuration["MongoDB:NotificacoesCollection"]);
    }

    public void ConsumirPedidos()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _configuration["RabbitMQ:QueueName"], durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonPedido = Encoding.UTF8.GetString(body);
            var pedido = JsonConvert.DeserializeObject<Pedido>(jsonPedido);

            ArmazenarPedidoNoMongoDB(pedido);

            Console.WriteLine($"Pedido recebido e armazenado: {pedido.Id}");

            // Notificar entregadores
            NotificarEntregadores(pedido);
        };

        channel.BasicConsume(queue: _configuration["RabbitMQ:QueueName"], autoAck: true, consumer: consumer);

        Console.WriteLine("Aguardando pedidos. Pressione [Enter] para sair.");
        Console.ReadLine();
    }

    private void ArmazenarPedidoNoMongoDB(Pedido pedido)
    {
        _pedidoCollection.InsertOne(pedido);
    }

    var pedidoConsumer = serviceProvider.GetRequiredService<PedidoConsumer>();
    pedidoConsumer.ConsumirPedidos();

    private void NotificarEntregadores(Pedido pedido)
    {
        // Lógica para identificar entregadores aptos
        var entregadoresAptos = ObterEntregadoresAptos();

        foreach (var entregador in entregadoresAptos)
        {
            var notificacao = new NotificacaoPedido
            {
                EntregadorId = entregador.Id,
                Mensagem = $"Novo pedido disponível: {pedido.Id}",
                DataNotificacao = DateTime.Now
            };

            _notificacaoCollection.InsertOne(notificacao);
        }
    }

    private List<Entregador> ObterEntregadoresAptos()
    {
        // Lógica para obter entregadores aptos (exemplo: locação ativa e sem pedido aceito)
    }

}
