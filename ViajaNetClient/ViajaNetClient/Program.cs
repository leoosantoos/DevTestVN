﻿using System;

namespace MessagingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            Console.WriteLine(" Pressione ESC para parar. ");
            client.Send();
        }
    }
}
