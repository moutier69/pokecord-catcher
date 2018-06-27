using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PokecordCatcherBot
{
    public static class Logger
    {
        private static readonly ConcurrentQueue<string> loggerQueue = new ConcurrentQueue<string>();
        private static bool isLogging;

        private async static Task ProcessLogs()
        {
            while (true)
            {
                while (loggerQueue.Count > 0)
                {
                    if (loggerQueue.TryDequeue(out string log))
                    {
                        File.AppendAllText("log.txt", log);
                    }
                }

                await Task.Delay(100);
            }
        }

        public async static Task StartLogging()
        {
            if (!isLogging)
            {
                Task.Run(async () => await ProcessLogs());
                isLogging = true;
            }
        }

        public static void Log(string msg)
        {
            if (isLogging)
                loggerQueue.Enqueue($"[{DateTime.Now}] {msg}{Environment.NewLine}");
        }
    }
}
