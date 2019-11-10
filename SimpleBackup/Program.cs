using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Serilog;

using SimpleBackup.Configuration;

namespace SimpleBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs\\.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Reading configuration ...");
                var config = Config.Get();
                new BackupRunner(config).Run();
            }
            catch (ConfigException ex)
            {
                Log.Error($"{ex}");

            }
            catch (Exception ex)
            {
                Log.Error($"General error. {ex}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
