using System;

namespace MessagingReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var receiver = new Receiver();
            Console.WriteLine(" Pressione ESC para sair. ");
            receiver.Receive();
        }
    }
}
