using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

//receiver
var factory = new ConnectionFactory() { HostName = "192.168.1.24", UserName = "root", Password = "123456", Port = 5672 };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    //channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Topic,durable:true);

    //var queueName = channel.QueueDeclare("", exclusive: true).QueueName;

    //channel.QueueBind(queue: queueName,
    //                  exchange: "logs",
    //                  routingKey: "");

    //Console.WriteLine(" [*] Waiting for logs.");

    //var consumer = new EventingBasicConsumer(channel);
    //consumer.Received += (model, ea) =>
    //{
    //    var body = ea.Body.ToArray();
    //    var message = Encoding.UTF8.GetString(body);
    //    Console.WriteLine(" [x] {0}", message);
    //};
    //channel.BasicConsume(queue: queueName,
    //                     autoAck: true,
    //                     consumer: consumer);

    //channel.QueueDeclare(queue: "task_queue",
    //                             durable: true,
    //                             exclusive: false,
    //                             autoDelete: false,
    //                             arguments: null);
    //var properties = channel.CreateBasicProperties();
    //properties.Persistent = true;

    channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);

        int dots = message.Split('.').Length - 1;
        Thread.Sleep(dots * 1000);

        Console.WriteLine(" [x] Done");

        // Note: it is possible to access the channel via
        //       ((EventingBasicConsumer)sender).Model here
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    };
    channel.BasicConsume(queue: "task_queue",
                         autoAck: false,
                         consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();

}