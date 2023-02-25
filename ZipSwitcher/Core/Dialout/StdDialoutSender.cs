using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZipSwitcher.Core.Dialout
{
    class StdDialoutSender : IDialoutSender
    {
        public async Task NotifyAsync(string subject, string message)
        {
            ConsoleLogger.Log("ALERT", subject + ": " + message);
        }
    }
}
