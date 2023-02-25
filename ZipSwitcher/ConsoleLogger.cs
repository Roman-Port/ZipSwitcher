using System;
using System.Collections.Generic;
using System.Text;

namespace ZipSwitcher
{
    static class ConsoleLogger
    {
        private static object mutex = new object();

        public static void Log(string topic, string message)
        {
            lock (mutex)
                Console.WriteLine($"[{DateTime.Now:M/d/yy HH:mm:ss}] [{topic}] {message}");
        }
    }
}
