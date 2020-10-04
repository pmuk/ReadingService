using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingService
{
    public class CsvPrinter
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string _filePath;
        private readonly string HEADER = "Local Time,Volume";
        private readonly string FILE_PATH = "file";
        private readonly string DEFAULT = "c:\\tmp\\PowerPosition.csv";
        public CsvPrinter(DateTime date)
        {
            _log.Info("Extracting file information from the configuration.");

            string filePath = null;
            try
            {
                filePath = ConfigurationManager.AppSettings[FILE_PATH];
            }
            catch (ConfigurationErrorsException e)
            {
                _log.Error($"unable to read the value of {FILE_PATH}", e);
                filePath = DEFAULT;
            }

            _filePath = Path.Combine(
                            Path.GetDirectoryName(filePath), 
                            $"{Path.GetFileNameWithoutExtension(filePath)}{date:_yyyyMMdd_HHmm}{Path.GetExtension(filePath)}"
                            );
        }
        public void Print(IEnumerable<IAggregatePeriod> aggregates)
        {
            _log.Info("Extracting information for CSV printing.");
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine(HEADER);
                foreach (var agg in aggregates)
                {
                    builder.AppendLine($"{agg.Period:HH:mm},{agg.Volume}");
                }
                _log.Info("Writing the csv.");
                File.WriteAllText(_filePath, builder.ToString());
                _log.Info("Writing done.");
            }
            catch(Exception e)
            {
                _log.Error("Exception caught while printing the results", e);
            }
        }
    }
}
