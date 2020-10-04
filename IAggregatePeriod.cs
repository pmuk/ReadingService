using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingService
{
    public interface IAggregatePeriod
    {
        DateTime Period { get; }
        double Volume { get; }
    }
}
