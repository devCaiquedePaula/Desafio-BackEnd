using System;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main()
    {
        // Conectar ao MongoDB
        string mongoConnectionString = "sua_string_de_conexao_mongodb";
        MongoClient mongoClient = new MongoClient(mongoConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase("seu_banco_de_dados");

        // Conectar ao RabbitMQ
        string rabbitMQConnectionString = "sua_string_de_conexao_rabbitmq";
        var factory = new ConnectionFactory() { Uri = new Uri(rabbitMQConnectionString) };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Configurar fila no RabbitMQ
            channel.QueueDeclare(queue: "sua_fila", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Consumir mensagens da fila
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Mensagem Recebida: {message}");

                // Adicionar lógica para processar a mensagem, por exemplo, salvar no MongoDB
                SalvarNoMongoDB(message);
            };

            channel.BasicConsume(queue: "sua_fila", autoAck: true, consumer: consumer);

            Console.WriteLine("Aguardando mensagens. Pressione [Enter] para sair.");
            Console.ReadLine();
        }
    }

    static void SalvarNoMongoDB(string message)
    {
        // Adicionar lógica para salvar no MongoDB
        // var collection = mongoDatabase.GetCollection<BsonDocument>("sua_colecao");
        // var document = BsonDocument.Parse(message);
        // collection.InsertOne(document);
    }
}

