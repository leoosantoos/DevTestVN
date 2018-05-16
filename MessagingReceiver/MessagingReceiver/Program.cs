using System;

namespace MessagingReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var receiver = new Receiver();
            Console.WriteLine(" Pressione Enter para iniciar e ESC para encerrar recebimento de mensagens.\n\n ");
            receiver.Receive();
            receiver.MapBehaviour();
        }
    }
}
