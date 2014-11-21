using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DealabsParser.Model;
using DealabsParser.Parser;

namespace DealabsAlertTest
{
    [TestClass]
    public class DealabsItemTest
    {
        [TestMethod]
        public void ParserDegreTest()
        {
            Uri test = new Uri(@"\Resources\DealabsItem.html", UriKind.Relative);
            //DealabsItem Item = new DealabsItem(@"\Resources\DealabsItem.html", "titre", DateTime.Now, "Description");
            DealabsItem Item = new DealabsItem();
            DealabsItemParser Parser = new DealabsItemParser(@"\Resources\DealabsItem.html");
            Item = Parser.parserDeal(Item);
            String Expected = " 72°";
            String Actual = Item.Degre;
            Assert.AreEqual(Expected, Actual);
        }
    }
}
