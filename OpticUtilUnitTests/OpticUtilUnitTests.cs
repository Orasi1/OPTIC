using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpticUtil;

namespace PerformanceCounterUtilityUnitTests
{
    [TestClass]
    public class OpticUtilUnitTests
    {
        [TestMethod]
        public void DeleteCounterCategory_NoInstance()
        {
            try
            {
                Counter.DeleteCounterCategory(@"HP LoadRunner Performance");
            }
            catch (System.UnauthorizedAccessException)
            {
            }
        }

        [TestMethod]
        public void DeleteCounterCategory_Prefix()
        {
            //Counter.DeleteCounterCategory("\\HP LoadRunner Performance");
        }

        [TestMethod]
        public void ResetCounter1()
        {
            try
            {
                Counter.ResetCounter(@"HP LoadRunner Performance(MyWebService)", 0);
            }
            catch(Exception e)
            {
                string strExpectedMessage = "Counter instance \"MyWebService\" provided but name not specified.";
                Assert.AreNotSame(e.Message, strExpectedMessage, "Expected error not found.");
            }
        }

        [TestMethod]
        public void ResetCounterContainsCounterName()
        {
            try
            {
                Counter.ResetCounter(@"HP LoadRunner Performance(MyWebService)\Transactions/Second", 0);
            }
            catch (Exception e)
            {
                string strExpectedMessage = "Counter name : \"Transactions/Second\" not understood. Please use \"Count\", \"Rate/Sec\", or \"Average\".";
                Assert.AreEqual(e.Message, strExpectedMessage, "Expected error not found.");
            }
        }

        [TestMethod]
        public void IncrementCounter1()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService)\Count", 1);
        }

        [TestMethod]
        public void GetCounter1()
        {
            Counter.ResetCounter(@"HP LoadRunner Performance(MyWebService)\Count", 0);
            int expected = 3;
            int counter;
            for (counter = 0; counter < expected; counter++)
            {
                Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService)\Count", 1);
            }
            long returned = Counter.GetCounter(@"HP LoadRunner Performance(MyWebService)\Count");
            Assert.AreEqual(counter, returned, string.Format("Values not equal: {0} != {1}", counter, returned));
        }

        [TestMethod]
        public void GetCounter2()
        {
            double d = 1.1234512451245135;
            float f = (float)d;
            long l = (long)f;
            System.Single s = (float)d;

            long returned = Counter.GetCounter(@"Processor Information(_Total)\% Processor Time");
            returned = Counter.GetCounter(@"Process(services)\Handle Count");
            Assert.AreNotEqual(0, returned, string.Format("Values not equal: {0} != {1}", 0, returned));
        }

        [TestMethod]
        public void IncrementCounterInstance1()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService 1)\Count", 1);
        }
        [TestMethod]
        public void IncrementCounterInstance2()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService 2)\Count", 1);
        }
        [TestMethod]
        public void IncrementCounterInstance3()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService 3)\Count", 1);
        }
        [TestMethod]
        public void IncrementCounterInstance4()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService 4)\Count", 1);
        }

        [TestMethod]
        public void IncrementRateCounter1()
        {
            int counter = 1;
            int iterations = 10;
            int iterator;
            for (iterator = 0; iterator < iterations; iterator++)
            {
                Counter.IncrementCounter(@"HP LoadRunner Performance(MyWebService)\Rate/Sec", 1);
            }
            long returned = Counter.GetCounter(@"HP LoadRunner Performance(MyWebService)\Rate/Sec");
            Assert.AreEqual(counter, returned, string.Format("Values not equal: {0} != {1}", counter, returned));
        }

        [TestMethod]
        public void DeleteCounterCategory2()
        {
            try
            {
                Counter.DeleteCounterCategory(@"HP LoadRunner Performance(MyWebService)\Count");
            }
            catch (System.UnauthorizedAccessException)
            {
            }

        }

        [TestMethod]
        public void ResetCounter_WithMachineName()
        {
            Counter.ResetCounter(@"HP LoadRunner Performance(<computername> MyWebService)\Count", 0);
        }

        [TestMethod]
        public void IncrementCounter_WithMachineName1()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(<computername> MyWebService)\Count", 1);
        }
        [TestMethod]
        public void IncrementCounter_WithMachineName2()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(<computername> MyWebService)\Count", 1);
        }
        [TestMethod]
        public void IncrementCounter_WithMachineName3()
        {
            Counter.IncrementCounter(@"HP LoadRunner Performance(<computername> MyWebService)\Count", 1);
        }

    }
}
