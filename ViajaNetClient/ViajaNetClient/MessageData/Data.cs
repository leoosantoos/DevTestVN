using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ViajaNetClient
{
    public class Data
    {
        public string ip { get; set; }
        public string log { get; set; }
        public string browser { get; set; }
        public string data { get; set; }
        
        private List<string> logList { get; set; }
        private List<string> browserList { get; set; }

        public Data()
        {
            SetIP();
            logList = new List<string>();
            browserList = new List<string>();

            logList.Add("www.uol.com.br");
            logList.Add("news.google.com.br");
            logList.Add("www.9gag.com");
            logList.Add("twitch.tv");

            browserList.Add("Chrome");
            browserList.Add("Firefox");
        }

        private void SetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.ip = ip.ToString();
                }
            }
        }

        public string GetLog()
        {
            var rand = new Random();
            return logList[rand.Next(logList.Count)];
        }

        public string GetBrowser()
        {
            var rand = new Random();
            return browserList[rand.Next(browserList.Count)];
        }
    }
}
