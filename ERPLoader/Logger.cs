using System;
using System.IO;

namespace ERPLoader
{
    static class Logger
    {
        private static readonly System.Threading.ReaderWriterLockSlim fileWriteLock = new();
        private static DateTime Now => DateTime.Now;

        public enum MessageType
        {
            Log, Warning, Error
        }

        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[~] " + msg);

            FileWrite(msg, MessageType.Log);
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] " + msg);
            Console.ForegroundColor = ConsoleColor.White;

            FileWrite(msg, MessageType.Warning);
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[X] " + msg);
            Console.ForegroundColor = ConsoleColor.White;

            FileWrite(msg, MessageType.Error);
        }

        public static void FileWrite(string msg, MessageType messageType = MessageType.Log)
        {
            Directory.CreateDirectory("Logs");
            string logFilePath = Path.Combine("Logs", $"{Now.Year}-{Now.Month:D2}-{Now.Day:D2}.log");

            fileWriteLock.EnterWriteLock();

            try
            {
                File.AppendAllText(logFilePath, $"[{Now.Year}/{Now.Month:D2}/{Now.Day:D2}][{Now.Hour:D2}:{Now.Minute:D2}] [{messageType}] " + msg + "\r\n");
            }
            finally
            {
                fileWriteLock.ExitWriteLock();
            }
        }

        public static void NewLine()
        {
            Console.WriteLine();
            File.AppendAllText("logs.txt", "\r\n");
        }
    }
}
