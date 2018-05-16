using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MessagingClient
{
    class Client
    {
        public void Send ()
        {
            while (Console.ReadKey(true).Key != ConsoleKey.Enter && Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
            }

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "messaging_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = GetLog();

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "",
                                             routingKey: "messaging_queue",
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine(" Mensagem enviada: {0}", message);

                        System.Threading.Thread.Sleep(500);

                        message = GetLog();
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }            
        }

        internal string GetLog()
        {
            Random rand = new Random();
            var logger = new Data();

            logger.log = logger.GetLog();
            logger.browser = logger.GetBrowser();
            logger.timeSpent = rand.Next(60);
            logger.date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return JsonConvert.SerializeObject(logger);
        }
    }
}
