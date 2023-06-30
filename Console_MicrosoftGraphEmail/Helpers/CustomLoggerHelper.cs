using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Helpers
{
    public static class CustomLoggerHelper
    {
        public static void WriteNewLog(string logFilePath, string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                DateTime dateTime = DateTime.UtcNow;

                string log = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] {message}";
                Console.WriteLine(log);

                writer.WriteLine(log);
            }
        }


        public static void WriteInLog(string logFilePath, string message, bool error)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                DateTime dateTime = DateTime.UtcNow;
                string log = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] {message}";

                if (error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(log);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(log);
                }

                Console.ForegroundColor = ConsoleColor.White;
                writer.WriteLine($"{log}");
            }
        }

        public static void StartNewLog(string fileLogPath)
        {
            using (StreamWriter writer = new StreamWriter(fileLogPath, false))
            {
                DateTime dateTime = DateTime.UtcNow;

                string toWrite = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] New Log... ";

                Console.WriteLine(toWrite);
                writer.WriteLine($"{toWrite}");
                //writer.WriteLine($"");
            }
        }
    }
}
