using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingService
{
    public class AggregatePeriod : IAggregatePeriod
    {
        DateTime _period;
        double _volume;
        public DateTime Period => _period;

        public double Volume => _volume;

        public AggregatePeriod(int period, double volume) 
        {
            _period = new DateTime(1, 1, 1, (period + 22) % 24, 0, 0);
            _volume = volume;
        }

        public void AddVolume(double volume)
        {
            _volume += volume;
        }
    }
}
