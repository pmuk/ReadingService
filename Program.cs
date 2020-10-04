using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadingService
{
    class Program
    {
        public const string ServiceName = "ReadingService";

        public class Service : ServiceBase
        {
            Timer _timer;
            private static readonly string TICK = "tick";
            private static readonly int _tick = 0;
            public Service()
            {
                ServiceName = Program.ServiceName;
                string cfg = null;
                try
                {
                    cfg = ConfigurationManager.AppSettings[TICK];
                }
                catch(ConfigurationErrorsException e)
                {
                    _log.Error($"unable to read the value of {TICK}", e);
                }

                if (!int.TryParse(cfg, out int _tick))
                    _tick = 60000;
            }

            private void OnTick(object state)
            {
                Program._log.Info("trigerring generation of readings");
                
                var reading = new Reading();
                reading.GenerateReadings(DateTime.Now);
                
                Program._log.Info("end of OnTick.");
            }

            protected override void OnStart(string[] args)
            {
                _log.Info("Starting the service.");
                _timer = new Timer(OnTick, "OnTick", 0, _tick);
            }

            protected override void OnStop()
            {
                _log.Info("ending the service.");
            }
        }

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            // running as service
            using (var service = new Service())
                    ServiceBase.Run(service);
        }
    }
}
