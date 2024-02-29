# Sistema de Gerenciamento de Aluguel de Motos

Este projeto é um sistema de gerenciamento de aluguel de motos e entrega por meio de uma plataforma online. Ele foi construído utilizando .NET Core, MongoDB como banco de dados NoSQL, e RabbitMQ para comunicação assíncrona.

## Funcionalidades

### Entregadores

-   Cadastro de entregadores com informações detalhadas.
-   Consulta individual de entregadores.
-   Notificação de pedidos disponíveis.

### Admin

-   Cadastro de motos.
-   Consulta de motos existentes.

### Pedidos

-   Criação de pedidos.
-   Notificação assíncrona de pedidos disponíveis aos entregadores.
-   Aceitação e entrega de pedidos por parte dos entregadores.
-   Cálculo de custos de locação.

## Como Executar

1. **Configuração do Ambiente:**

    - Certifique-se de ter o .NET Core SDK instalado.
    - Configure um banco de dados MongoDB.
    - Instale e configure o RabbitMQ.

2. **Configurações:**

    - Configure as strings de conexão para o MongoDB e RabbitMQ no arquivo `appsettings.json`.

3. **Execução:**

    - Execute o projeto usando `dotnet run` no terminal.

4. **Teste:**
    - Use ferramentas como o Postman para testar os endpoints.

## Estrutura do Projeto

-   `Models`: Contém as classes de modelo para Entregador, Moto, Pedido e NotificaçãoPedido.
-   `Controllers`: Contém os controladores para Entregador e Admin.
-   `Services`: Contém o serviço PedidoService para gerenciar pedidos.
-   `Consumers`: Contém o consumidor PedidoConsumer para processar notificações assíncronas.

## Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para abrir problemas ou enviar solicitações de pull.

## Licença

Este projeto é licenciado sob a Licença MIT - veja o arquivo [LICENSE.md](LICENSE.md) para detalhes.
