using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.N1QL;
using Couchbase.Views;
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
        private int counter = 0;

        public void Receive()
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

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var authentication = new Couchbase.Authentication.PasswordAuthenticator("default", "password");
                Cluster.Authenticate(authentication);

                using (var bucket = Cluster.OpenBucket("MessagingServer"))
                {
                    var countResult = bucket.Query<dynamic>("SELECT COUNT(0) as `result` FROM `MessagingServer`");
                    counter = (int)(JsonConvert.DeserializeObject(countResult.Rows[0].ToString())).result;

                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            var consumidor = new EventingBasicConsumer(channel);
                            consumidor.Received += (model, e) =>
                            {
                                var body = e.Body;
                                var message = System.Text.Encoding.UTF8.GetString(body);

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
        }

        internal void MapBehaviour()
        {
            UserActivity ua = new UserActivity();

            using (var bucket = Cluster.OpenBucket("MessagingServer"))
            {
                var query = new ViewQuery().From("mapreduce", "ClientActivity")
                    .GroupLevel(1)
                    .Reduce(true);

                var viewResult = bucket.Query<dynamic>(query);

                Console.Clear();
                Console.WriteLine("WebSite\tTempo Total(s)");

                foreach (var line in viewResult.Rows)
                {
                    if(ua.MostTime == 0 && ua.LeastTime == 0)
                    {
                        ua.MostTime = Convert.ToInt32(line.Value);
                        ua.LeastTime = Convert.ToInt32(line.Value);
                        ua.LongAcess = line.Key;
                        ua.ShortAcess = line.Key;
                    }
                    else if(ua.MostTime < Convert.ToInt32(line.Value))
                    {
                        ua.MostTime = Convert.ToInt32(line.Value);
                        ua.LongAcess = line.Key;
                    }
                    else if(ua.LeastTime > Convert.ToInt32(line.Value))
                    {
                        ua.LeastTime = Convert.ToInt32(line.Value);
                        ua.ShortAcess = line.Key;
                    }
                    Console.WriteLine(line.Key + "\t" + line.Value);
                }

                query = new ViewQuery().From("mapreduce", "MostAcessed")
                    .GroupLevel(1)
                    .Reduce(true);

                viewResult = bucket.Query<dynamic>(query);

                Console.WriteLine("\n\nWebSite\tTotal de acessos");
                foreach (var line in viewResult.Rows)
                {
                    if (ua.MostAcessed.Equals(string.Empty) && ua.LeastAcessed.Equals(string.Empty))
                    {
                        ua.MostAcessed = line.Key;
                        ua.LeastAcessed = line.Key;
                        ua.TopAcess = Convert.ToInt32(line.Value);
                        ua.BottomAcess = Convert.ToInt32(line.Value);
                    }
                    else if (ua.TopAcess < Convert.ToInt32(line.Value))
                    {
                        ua.TopAcess = Convert.ToInt32(line.Value);
                        ua.MostAcessed = line.Key;
                    }
                    else if (ua.BottomAcess > Convert.ToInt32(line.Value))
                    {
                        ua.BottomAcess = Convert.ToInt32(line.Value);
                        ua.LeastAcessed = line.Key;
                    }
                    Console.WriteLine(line.Key + "\t" + line.Value);
                }

                Console.WriteLine("\n\nMais acessado: {0} - Total acessos: {1}" +
                    "\nMenos acessado: {2} - Total acessos: {3}" +
                    "\nAcesso mais longo: {4} - Tempo total (s): {5}" +
                    "\nAcesso mais curto: {6} - Tempo total (s): {7}", ua.MostAcessed, ua.TopAcess, ua.LeastAcessed, ua.BottomAcess, ua.LongAcess, ua.MostTime, ua.ShortAcess, ua.LeastTime);
            }

            Console.WriteLine("\n\nPressione ESC para sair. ");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {

            }
        }

        private void Work(string messageJson)
        {
            Data data = JsonConvert.DeserializeObject<Data>(messageJson);

            var authentication = new Couchbase.Authentication.PasswordAuthenticator("default", "password");
            Cluster.Authenticate(authentication);

            using (var bucket = Cluster.OpenBucket("MessagingServer"))
            {
                bucket.Insert(counter.ToString(), data);
                counter++;
            }
        }
    }
}