using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpticUtil;

namespace PerformanceCounterUtilityConsole
{
    class OpticUtilConsole
    {
        static void Main(string[] args)
        {
            string myCategory = "Prudent Counters";
            string myInstance;

            Counter.DeleteCounterCategory(myCategory);

            myInstance = "Instance1";
            Counter.ResetCounter(string.Format("{0}({1})", myCategory, myInstance), 0);
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 1);

            myInstance = "Instance2";
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 2);

            myInstance = "Instance3";
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 3);
        }
    }
}
