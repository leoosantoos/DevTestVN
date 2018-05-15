using Couchbase;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingReceiver
{
    class Receiver
    {

        private static readonly Cluster Cluster = new Cluster();
        private int count = 0;

        public void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "messaging_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        var consumidor = new EventingBasicConsumer(channel);
                        consumidor.Received += (model, e) =>
                        {
                            var body = e.Body;
                            var message = Encoding.UTF8.GetString(body);

                            Console.WriteLine(" Mensagem recebida: {0}", message);

                            Work(message);

                            channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        };

                        channel.BasicConsume(queue: "messaging_queue",
                                             autoAck: false,
                                             consumer: consumidor);

                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
        }

        private void Work(string messageJson)
        {         
            Data data = JsonConvert.DeserializeObject<Data>(messageJson);
            
            var authentication = new Couchbase.Authentication.PasswordAuthenticator("dragon", "leo14p810.");
            Cluster.Authenticate(authentication);

            using (var bucket = Cluster.OpenBucket("MessagingServer"))
            {
                var message = new Document<Data>()
                {
                    Content = new Data()
                    {
                        ip = data.ip,
                        log = data.log,
                        browser = data.browser,
                        timeSpent = data.timeSpent,
                        date = data.date
                    }
                };

                bucket.Insert(count.ToString(), message);
                count++;
            }
        }
    }
}
