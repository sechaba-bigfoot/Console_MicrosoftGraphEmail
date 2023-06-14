using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Helpers
{
    public static class CustomLogger
    {
        public static void WriteNewLog(string logFilePath, string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                Console.WriteLine(message);

                DateTime dateTime = DateTime.UtcNow;
                writer.WriteLine($"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] Log");
                writer.WriteLine(">>------------------------------------------------");
                writer.WriteLine($"{message}");
            }
        }

        public static void WriteInLog(string logFilePath, string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                Console.WriteLine(message);

                writer.WriteLine($"{message}");
            }
        }

        public static void StartNewLog(string fileLogPath)
        {
            using (StreamWriter writer = new StreamWriter(fileLogPath, false))
            {
                DateTime dateTime = DateTime.UtcNow;

                string toWrite = $"NEW LOG STARTED [{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] ||||||||||||||||||||||||||||||||||||||||||||||||||||||| ";

                Console.WriteLine(toWrite);
                writer.WriteLine($"{toWrite}");
            }
        }
    }
}
