using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingReceiver
{
    class UserActivity
    {
        public string MostAcessed { get; set; }
        public string LeastAcessed { get; set; }
        public string LongAcess { get; set; }
        public string ShortAcess { get; set; }
        public int MostTime { get; set; }
        public int LeastTime { get; set; }
        public int TopAcess { get; set; }
        public int BottomAcess { get; set; }

        public UserActivity()
        {
            MostTime = 0;
            LeastTime = 0;
            MostAcessed = string.Empty;
            LeastAcessed = string.Empty;
            TopAcess = 0;
            BottomAcess = 0;
            LongAcess = string.Empty;
            ShortAcess = string.Empty;
        }
    }
}
