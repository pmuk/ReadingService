using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingService
{
    public static class ConfigurationReader
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string MAX_RETRY = "maxRetry";
        
        static readonly string FILE_PATH = "file";
        static readonly string DEFAULT = "c:\\tmp\\PowerPosition.csv";
        private static readonly string TICK = "tick";
        
        static int _tick;
        static string _filePath;
        static int _maxRetry;

        static ConfigurationReader()
        {
            string maxRetry = null;
            try
            {
                maxRetry = ConfigurationManager.AppSettings[MAX_RETRY];
            }
            catch (ConfigurationErrorsException e)
            {
                _log.Error($"unable to read the value of {MAX_RETRY}", e);
            }

            if (!int.TryParse(maxRetry, out _maxRetry))
                _maxRetry = 1;

            _log.Info("Extracting file information from the configuration.");

            try
            {
                _filePath = ConfigurationManager.AppSettings[FILE_PATH];
            }
            catch (ConfigurationErrorsException e)
            {
                _log.Error($"unable to read the value of {FILE_PATH}", e);
                _filePath = DEFAULT;
            }

            string cfg = null;
            try
            {
                cfg = ConfigurationManager.AppSettings[TICK];
            }
            catch (ConfigurationErrorsException e)
            {
                _log.Error($"unable to read the value of {TICK}", e);
            }

            if (!int.TryParse(cfg, out _tick))
                _tick = 60000;
        }

        public static int MaxRetry => _maxRetry;

        public static int Tick
        {
            get { return _tick; }
        }
        
        public static string GetFilePath(DateTime date)
        {
            return Path.Combine(
                            Path.GetDirectoryName(_filePath),
                            $"{Path.GetFileNameWithoutExtension(_filePath)}{date:_yyyyMMdd_HHmm}{Path.GetExtension(_filePath)}"
                            );
        }
    }
}
