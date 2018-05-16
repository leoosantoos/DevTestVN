using System;

namespace MessagingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            Console.WriteLine(" Pressione Enter para iniciar e ESC para parar o envio de mensagens.\n\n ");
            client.Send();
        }
    }
}
