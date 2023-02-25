using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZipSwitcher.Core
{
    interface IDialoutSender
    {
        /// <summary>
        /// Sends a notification with the specified subject and message.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task NotifyAsync(string subject, string message);
    }
}
