using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier
{
    class Logger
    {

        public static void Error(object message)
        {
            Console.WriteLine("[ERROR] " + message.ToString());
        }

        public static void Info(object message)
        {
            Console.WriteLine("[INFO] " + message.ToString());
        }

    }
}
