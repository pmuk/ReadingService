using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ReadingService
{
    public class Reading
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        PowerService _powerService;
        const string MAX_RETRY = "maxRetry";
        int _maxRetry = 0;
        public Reading() 
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

            _log.Info("Creating the access to the reading service");
            _powerService = new PowerService();
        }

        async Task<IEnumerable<PowerTrade>> GetTrades(DateTime date)
        {
            int retry = 0;
            IEnumerable<PowerTrade> listTrade = new List<PowerTrade>();
        ACCESS_POWER:
            try
            {
                listTrade = await _powerService.GetTradesAsync(date);
            }
            catch (PowerServiceException ex)
            {
                _log.Error("Error retrieving the power trades", ex);
                ++retry;
                if (retry <= _maxRetry)
                    goto ACCESS_POWER;
                throw new Exception("Too many PowerService exceptions, max retry reached.");
            }
            catch (Exception exc)
            {
                _log.Error("Unexpected error in getting trades", exc);
                throw;
            }

            return listTrade;
        }

        public async Task GenerateReadings(DateTime date) 
        {
            _log.Info("Accessing trades.");
            
            try
            {
                var listTrade = await GetTrades(date);
                var aggregatedPeriod = new AggregatePeriod[24];
                for (int i = 0; i < 24; ++i)
                {
                    aggregatedPeriod[i] = new AggregatePeriod(i + 1, 0);
                }
                _log.Info("Processing trades.");
                foreach (var trade in listTrade)
                {
                    for (int i = 0; i < trade.Periods.Length; ++i)
                    {
                        aggregatedPeriod[i].AddVolume(trade.Periods[i].Volume);
                    }
                }

                _log.Info("Printing.");
                var printer = new CsvPrinter(date);
                printer.Print(aggregatedPeriod);
                _log.Info("Printing done.");
            }
            catch(Exception ex)
            {
                _log.Error("Exception caught in GenerateReading", ex);
                throw;
            }
        }
    }
}
