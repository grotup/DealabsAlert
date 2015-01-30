using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DealabsDatabase;

namespace DealabsAlertTest.Database
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Database1()
        {
            SQLiteDealabsData Test = new SQLiteDealabsData("dealabsAlert", "deals");
        }
    }
}
