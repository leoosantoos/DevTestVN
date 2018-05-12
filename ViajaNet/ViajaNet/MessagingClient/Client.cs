using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ViajaNetClient
{
    class Client
    {
        public void Send ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "viajanet_queue",
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
                                             routingKey: "viajanet_queue",
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine(" Mensagem enviada: {0}", message);

                        System.Threading.Thread.Sleep(2000);

                        message = GetLog();
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }            
        }

        internal string GetLog()
        {
            var logger = new Data();

            logger.log = logger.GetLog();
            logger.browser = logger.GetBrowser();
            logger.data = DateTime.Now.ToString("ddMMyyyy HH:mm:ss");

            return JsonConvert.SerializeObject(logger);
        }
    }
}
