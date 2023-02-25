using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZipSwitcher.Core;

namespace ZipSwitcher
{
    class Switcher : IDisposable
    {
        public Switcher(string name, IDialoutSender dialout, ZipConnection zip, LogWriter logger)
        {
            this.name = name;
            this.dialout = dialout;
            this.zip = zip;
            this.logger = logger;
        }

        private readonly string name;
        private readonly IDialoutSender dialout;
        private readonly ZipConnection zip;
        private readonly LogWriter logger;

        private bool refreshErrored = false;
        private bool logErrored = false;
        
        public async Task ProcessAsync()
        {
            //Attempt to refresh Z/IP data
            try
            {
                await zip.UpdateAsync();
                refreshErrored = false;
            } catch (Exception ex)
            {
                //Send out an alert if this is the first time it has failed
                if (!refreshErrored)
                {
                    await dialout.NotifyAsync($"{name} Z/IP not responding", $"Could not refresh statistics for Z/IP \"{name}\". The software will not function until it is back online.\n\nTechnical information: {ex.Message}{ex.StackTrace}");
                    refreshErrored = true;
                }

                //Abort
                return;
            }

            //Attempt to write to log
            try
            {
                if (logger != null)
                {
                    logger.WriteLine(new object[]
                    {
                        DateTime.Now,
                        zip.Total,
                        zip.Buffered,
                        zip.Lost,
                        zip.Concealed,
                        zip.Dropped
                    });
                }
                logErrored = false;
            }
            catch (Exception ex)
            {
                //Send out an alert if this is the first time it has failed
                if (!logErrored)
                {
                    await dialout.NotifyAsync($"Failed to write to log", $"Could not write log info to the disk. Is it full?\n\nTechnical information: {ex.Message}{ex.StackTrace}");
                    logErrored = true;
                }
            }
        }

        public void Dispose()
        {
            //Close logger
            logger.Dispose();
        }
    }
}
