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
                DateTime dateTime = DateTime.UtcNow;

                string log = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] {message}";
                Console.WriteLine(log);

                writer.WriteLine(log);
            }
        }

        public static void WriteInLog(string logFilePath, string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                DateTime dateTime = DateTime.UtcNow;
                string log = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] {message}";
                Console.WriteLine(log);

                writer.WriteLine($"{log}");
            }
        }

        public static void StartNewLog(string fileLogPath)
        {
            using (StreamWriter writer = new StreamWriter(fileLogPath, false))
            {
                DateTime dateTime = DateTime.UtcNow;

                string toWrite = $"[{dateTime.ToShortDateString()} | {dateTime.ToShortTimeString()}] NEW LOG STARTED ";

                Console.WriteLine(toWrite);
                writer.WriteLine($"{toWrite}");
                writer.WriteLine($"");
            }
        }
    }
}
