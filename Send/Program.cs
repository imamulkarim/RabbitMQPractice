using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

//*******************   sender  **********************************
var factory = new ConnectionFactory() { HostName="192.168.1.24", UserName="root" , Password="123456" , Port=5672 };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

    //channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Topic ,durable:true);

    //var queueName = channel.QueueDeclare("", exclusive : true).QueueName;
    //channel.QueueBind(queue: queueName,
    //                  exchange: "logs",
    //                  routingKey: "");

    channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


    Console.WriteLine("Press [x] to exit.");

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    while (true)
    {
        string? message = Console.ReadLine();

        if (message is null)
        {
            continue;
        }
        if (message.ToLower().Equals("x"))
        {
            break;
        }

        var body = Encoding.UTF8.GetBytes(message);



        //channel.BasicPublish(exchange: "logs",
        //             routingKey: "",
        //             basicProperties: null,
        //             body: body);

        channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);

        Console.WriteLine(" [x] Sent {0}", message);

    }
}

