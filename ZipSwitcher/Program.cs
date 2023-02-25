using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using ZipSwitcher.Config;
using ZipSwitcher.Core;
using ZipSwitcher.Core.Dialout;

namespace ZipSwitcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Make sure a config file is specified and exists
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                ConsoleLogger.Log("INIT", "No config file is specified or it doesn't exist. Specify it with the first commandline argument. Exiting...");
                return;
            }

            //Open config file and sanity check it
            ConfigRoot config;
            try
            {
                config = JsonConvert.DeserializeObject<ConfigRoot>(File.ReadAllText(args[0]));
                if (config == null)
                    throw new ConfigVerifyException("Config is null.");
                config.Verify();
            } catch (ConfigVerifyException ex)
            {
                ConsoleLogger.Log("INIT", $"Config is not valid: {ex.Message} Exiting...");
                return;
            } catch (Exception ex)
            {
                ConsoleLogger.Log("INIT", $"Unexpected error opening config file: {ex.Message}{ex.StackTrace} Exiting...");
                return;
            }

            //Create dialout handler
            IDialoutSender dialout;
            if (config.Dialout == null)
            {
                ConsoleLogger.Log("INIT", "No dialout information provided. Only printing to standard output.");
                dialout = new StdDialoutSender();
            } else
            {
                dialout = new EmailDialoutSender(config.SiteName, config.Dialout);
                ConsoleLogger.Log("INIT", $"E-Mail dialout ready with {config.Dialout.Targets.Length} recipient(s).");
            }

            //Create all airchains
            Switcher[] switches = new Switcher[config.Airchains.Length];
            for (int i = 0; i < config.Airchains.Length; i++)
            {
                //Get
                ConfigAirchain airchainConfig = config.Airchains[i];

                //Create components
                ZipConnection zip = new ZipConnection(new Uri(airchainConfig.ZipUrl));
                LogWriter logger = null;
                if (airchainConfig.LogFilename != null)
                {
                    try
                    {
                        logger = new LogWriter(airchainConfig.LogFilename, new string[]
                        {
                            "Time",
                            "Total",
                            "Buffered",
                            "Lost",
                            "Concealed",
                            "Dropped"
                        });
                    } catch
                    {
                        ConsoleLogger.Log("INIT", "Could not open logger file on: \"" + airchainConfig.LogFilename + "\". Exiting...");
                        return;
                    }
                }

                //Create airchain object
                switches[i] = new Switcher(airchainConfig.Name, dialout, zip, logger);

                //Log
                ConsoleLogger.Log("INIT", $"\"{airchainConfig.Name}\" airchain ready.");
            }

            //Run
            Task.WaitAll(RunAsync(config.RefreshInterval, dialout, switches));

            //Dispose of all
            ConsoleLogger.Log("DEINIT", "Exiting...");
            foreach (var s in switches)
                s.Dispose();
        }

        private static async Task RunAsync(int interval, IDialoutSender dialout, Switcher[] switchers)
        {
            //Log
            ConsoleLogger.Log("INIT", "Ready!");

            //Enter loop
            try
            {
                while (true)
                {
                    //Start delay
                    Task delay = Task.Delay(interval);

                    //Process all
                    for (int i = 0; i < switchers.Length; i++)
                        await switchers[i].ProcessAsync();

                    //Wait for delay
                    await delay;
                }
            } catch (Exception ex)
            {
                //Unknown fatal error. Log and dialout
                string message = $"Unknown fatal error: {ex.Message}{ex.StackTrace} Exiting...";
                ConsoleLogger.Log("LOOP", message);
                await dialout.NotifyAsync("Fatal error", message);
            }
        }
    }
}
