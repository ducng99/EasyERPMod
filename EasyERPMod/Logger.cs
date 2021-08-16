using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyERPMod
{
    static class Logger
    {
        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[-] " + msg);
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] " + msg);
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[X] " + msg);
        }
    }
}
