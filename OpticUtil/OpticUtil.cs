using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
namespace OpticUtil
{
    public static class AppDynamicsRest
    {
	//https://docs.appdynamics.com/display/PRO41/Use+the+AppDynamics+REST+APi
        private static string RestUrl = "controller/rest/applications";
        private static string EventsUrl = "events";

        public static void CreateCustomEvent(string baseUrl, string credentials, string applicationName, string severity, string customEventType, string summary)
        {
            RestPost(
                baseUrl,
                credentials,
                applicationName,
                new Dictionary<string, string>() {
                        {"eventtype", "CUSTOM"},
                        {"severity", severity},
                        {"customeventtype", customEventType},
                        {"summary", summary},
                });
        }

        private static void RestPost(string baseUrl, string credentials, string applicationName, Dictionary<string, string> urlParams)
        {
            HttpClient httpClient = Authenticate(baseUrl, credentials);

            StringBuilder queryString = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in urlParams)
            {
                queryString.Append(string.Format("{0}={1}&", kvp.Key, kvp.Value));
            }
            var url = string.Format("{0}/{1}/{2}?{3}", RestUrl, applicationName, EventsUrl, queryString.ToString()).Replace(" ", "%20");

            //Todo: not sure why i have to create empty content
            System.Net.Http.HttpContent content = new StringContent("", UTF8Encoding.UTF8, "application/xml");
            HttpResponseMessage message = httpClient.PostAsync(url, content).Result;

            string result = message.Content.ReadAsStringAsync().Result;

            Console.WriteLine("RestPost()");
            if (message.IsSuccessStatusCode)
            {
                Console.WriteLine("RestPost succeeded with the following information:");
                Console.WriteLine(result);
            }
            else
            {
                StringBuilder exceptionText = new StringBuilder();
                exceptionText.AppendLine("RestPost failed with the following information:");
                exceptionText.AppendLine(string.Format("ReasonPhrase: {0}", message.ReasonPhrase));
                exceptionText.Append(string.Format("Result: {0}", result));
                throw new Exception(exceptionText.ToString());
            }
        }

        private static HttpClient Authenticate(string baseUrl, string credentials)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            byte[] cred = UTF8Encoding.UTF8.GetBytes(credentials);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

            HttpResponseMessage message = httpClient.GetAsync(RestUrl).Result;

            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
            }
            else
            {
                throw new Exception(string.Format("Error authenticating. URL: {0}. Reason phrase: {1}. Request message:{2}.", RestUrl, message.ReasonPhrase, message.RequestMessage));
            }
            return httpClient;
        }
    }

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