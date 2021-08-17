using System;
using System.IO;

namespace ERPLoader
{
    static class Logger
    {
        private static DateTime Now => DateTime.Now;

        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[~] " + msg);

            File.AppendAllText("logs.txt", $"[{Now.Day}-{Now.Month}][{Now.Hour}:{Now.Minute}] [~] " + msg + "\r\n");
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] " + msg);
            Console.ForegroundColor = ConsoleColor.White;

            File.AppendAllText("logs.txt", $"[{Now.Day}-{Now.Month}][{Now.Hour}:{Now.Minute}] [*] " + msg + "\r\n");
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[X] " + msg);
            Console.ForegroundColor = ConsoleColor.White;

            File.AppendAllText("logs.txt", $"[{Now.Day}-{Now.Month}][{Now.Hour}:{Now.Minute}] [X] " + msg + "\r\n");
        }
    }
}
