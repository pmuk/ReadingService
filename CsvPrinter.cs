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
        private readonly string HEADER = "Local Time,Volume";

        static object obj = new object();

        public void Print(IEnumerable<IAggregatePeriod> aggregates, DateTime date)
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
                var fileName = ConfigurationReader.GetFilePath(date);
                _log.Info($"Writing the csv : {fileName}");
                
                //in case the tick is < 1 min
                lock (obj)
                {
                    File.WriteAllText(fileName, builder.ToString());
                }
                _log.Info("Writing done.");
            }
            catch(Exception e)
            {
                _log.Error("Exception caught while printing the results", e);
            }
        }
    }
}
