using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace OpticUtil
{
    public static class Counter
    {

        public static void IncrementCounter(string performanceCounter, double value)
        {
            var perfCounter = ParseCounterPath(performanceCounter, true);
            CreatePerformanceCounterCategory(perfCounter);
            IncrementPerformanceCounter(perfCounter, value);
        }


        public static void ResetCounter(string performanceCounter, int value)
        {
            var perfCounter = ParseCounterPath(performanceCounter, true);
            CreatePerformanceCounterCategory(perfCounter);
            ResetPerformanceCounter(perfCounter, value);
        }

        public static void DeleteCounterCategory(string performanceCounter)
        {
            var perfCounter = ParseCounterPath(performanceCounter, true);
            DeletePerformanceCounterCategory(perfCounter);
        }

        public static long GetCounter(string performanceCounter)
        {
            var perfCounter = ParseCounterPath(performanceCounter, false);
            return GetPerformanceCounter(perfCounter);
        }

        private static void IncrementPerformanceCounter(PerfCounter perfCounter, double value)
        {
            PerformanceCounter myCounter;
            myCounter = new PerformanceCounter(perfCounter.CategoryName, perfCounter.CounterName, perfCounter.CounterInstanceName, false);
            myCounter.IncrementBy((long)value);
        }

        private static long GetPerformanceCounter(PerfCounter perfCounter)
        {
            PerformanceCounter myCounter;
            myCounter = new PerformanceCounter(perfCounter.CategoryName, perfCounter.CounterName, perfCounter.CounterInstanceName, true);

            //Todo: Find solution to getting counters correctly
            //CounterSample cs = myCounter.NextSample();
            //cs.RawValue
            
            //Get the value twice, The first iteration of he counter will always be 0, because it has nothing to compare to the last value.
            float nextValue = 0;
            for(int i = 0; i < 10; i++)
            {
                nextValue = myCounter.NextValue();
                if(nextValue > 0)
                {
                    break;
                }
                System.Threading.Thread.Sleep((i + 1)  * 10);
            }
            return (long)nextValue; //Losing the decimal place here, but havent figured out how to marshall float back to LR
        }


        private static void ResetPerformanceCounter(PerfCounter perfCounter, int value)
        {
            PerformanceCounter myCounter = 
                new PerformanceCounter(
                    perfCounter.CategoryName,
                    perfCounter.CounterName,
                    perfCounter.CounterInstanceName,
                    false);
            myCounter.RawValue = value;
        }

        private static void CreatePerformanceCounterCategory(PerfCounter perfCounter)
        {
            //Logger logger = new Logger(@"OpticUtil.log");
            //logger.WriteEntry("CreatePerformanceCounterCategory begin.", "Info", "OpticUtil");

            if (!PerformanceCounterCategory.Exists(perfCounter.CategoryName))
            {
                CounterCreationDataCollection counterData = new CounterCreationDataCollection();
                CounterCreationData counter;
                
                //We need to add all possible counter names even if they wont be utilized by the user because once the category is created we can't
                //go back and add an additional counter.
                var counterNames = GetCounterNames();
                foreach (CounterNameClass counterNameClass in counterNames)
                {
                    counter = new CounterCreationData();
                    counter.CounterName = counterNameClass.CounterName;
                    counter.CounterType = counterNameClass.CounterType;
                    counter.CounterHelp = counterNameClass.CounterHelp;
                    counterData.Add(counter);
                }

                // Create the category and pass the collection to it.
                PerformanceCounterCategory.Create(perfCounter.CategoryName, perfCounter.CategoryHelp, perfCounter.CategoryType, counterData);

            }

            //logger.WriteEntry("CreatePerformanceCounterCategory end.", "Info", "OpticUtil");
            //logger.LoggerClose();
        }
        
        private static void DeletePerformanceCounterCategory(PerfCounter perfCounter)
        {
            if (PerformanceCounterCategory.Exists(perfCounter.CategoryName))
            {
                PerformanceCounterCategory.Delete(perfCounter.CategoryName);
            }
        }

        /// <summary>
        /// Accepts Category name and instance name
        /// Note, we dont need the counter name because they are pre-defined
        /// User passes in a path in this format
        /// "HP LoadRunner Performance"
        /// "\\HP LoadRunner Performance"
        /// "\\HP LoadRunner Performance(My Web Service)"
        /// "HP LoadRunner Performance(My Web Service)"
        /// "HP LoadRunner Performance(<computername> My Web Service)"
        /// </summary>
        private static PerfCounter ParseCounterPath(string performanceCounter, bool restrictCounterName)
        {
            var perfCounter = new PerfCounter();

            string[] perfCounterParts = performanceCounter
                .Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            //CategoryName
            perfCounter.CategoryName = perfCounterParts[0]
                .Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (perfCounterParts[0].Contains("(") && perfCounterParts[0].Contains(")"))
            {
                //InstanceName
                perfCounter.CounterInstanceName = perfCounterParts[0]
                    .Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries)[1]
                    .Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries)[0];

                //If user wants the computername added to the instance name they can just add the following:
                //  <computername> myinstancename
                var index = perfCounter.CounterInstanceName.IndexOf("<computername>");
                if (index >= 0)
                {
                    perfCounter.CounterInstanceName = perfCounter.CounterInstanceName.Replace("<computername>", System.Net.Dns.GetHostName());
                }

                if (perfCounterParts.Length == 2)
                {
                    perfCounter.CounterName = perfCounterParts[1];
                    switch (perfCounter.CounterName.ToLower())
                    {
                        case "numberofitems":
                        case "count":
                            perfCounter.CounterType = PerformanceCounterType.NumberOfItems32;
                            break;
                        case "rateofcountspersecond":
                        case "rate/sec":
                            perfCounter.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                            break;
                        case "averagetimer":
                        case "average":
                            perfCounter.CounterType = PerformanceCounterType.AverageTimer32;
                            break;
                        default:
                            if (restrictCounterName)
                            {
                                throw new Exception(string.Format("Counter name : \"{0}\" not understood. Please use \"Count\", \"Rate/Sec\", or \"Average\".", perfCounter.CounterName));
                            }
                            break;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Counter instance \"{0}\" provided but name not specified.", perfCounter.CounterInstanceName));
                }
            }

            //Both of these are hard coded
            perfCounter.CounterHelp = "OpticUtil Counter help.";
            perfCounter.CategoryHelp = "OpticUtil Category help.";
            perfCounter.CategoryType = PerformanceCounterCategoryType.MultiInstance;

            return perfCounter;
        }

        private static List<CounterNameClass> GetCounterNames()
        {
            return new List<CounterNameClass> {
                new CounterNameClass("Count", "OpticUtil Count.", PerformanceCounterType.NumberOfItems32) ,
                new CounterNameClass("Rate/Sec", "OpticUtil Rate/Sec.", PerformanceCounterType.RateOfCountsPerSecond32), 
                new CounterNameClass("Average", "OpticUtil Average Timer.", PerformanceCounterType.AverageTimer32),
                new CounterNameClass("Average Base", "OpticUtil Average Base.", PerformanceCounterType.AverageBase)
            };
        }

    }//class

    public class PerfCounter
    {
        public string CategoryName { get; set; }
        public string CategoryHelp { get; set; }
        public PerformanceCounterCategoryType CategoryType { get; set; }
        public string CounterName { get; set; }
        public PerformanceCounterType CounterType { get; set; }
        public string CounterHelp { get; set; }

        public string CounterInstanceName { get; set; }

        public PerfCounter() { }
        public PerfCounter(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, string counterName, string counterInstanceName)
        {
            CategoryName = categoryName;
            CategoryHelp = categoryHelp;
            CategoryType = categoryType;
            CounterName = counterName;
            CounterInstanceName = counterInstanceName;
        }
    }//class

    public class CounterNameClass
    {
        public string CounterName { get; set; }
        public string CounterHelp { get; set; }
        public PerformanceCounterType CounterType { get; set; }

        public CounterNameClass(string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            CounterName = counterName;
            CounterHelp = counterHelp;
            CounterType = counterType;
        }
    }//class

}//namespace