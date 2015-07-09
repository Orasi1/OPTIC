using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpticUtil;
using System.IO;

namespace PerformanceCounterUtilityConsole
{
    class OpticUtilConsole
    {
        static void Main(string[] args)
        {
            string dir = args[0];
            VuGenUtilFunctions.ProcessDirectory(dir);
        }

        private static void TestCounters()
        {
            TestCounters();
            string myCategory = "My Counters";
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
