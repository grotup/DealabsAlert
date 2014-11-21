using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;
using DealabsParser.Parser;
using DealabsParser.Model;

namespace DealAlert.Test
{
    [TestFixture]
    class ParserTest
    {
        [Test]
        public void parserRSSTest()
        {
            DealabsRssParser parser = new DealabsRssParser("flux1.xml", 2, int.MaxValue);
            parser.updateItems();

            Assert.IsTrue(parser.GetList(string.Empty).Count == 27);
        }

        [Test]
        public void GetListSansFiltre()
        {
            DealabsRssParser parser = new DealabsRssParser("flux1.xml", 2, int.MaxValue);
            parser.updateItems();

            List<DealabsItem> ItemsFiltres = parser.GetList("");

            Assert.IsTrue(ItemsFiltres.Count == 27);
        }

        [Test]
        public void GetListAvecFiltre()
        {
            DealabsRssParser parser = new DealabsRssParser("flux1.xml", 2, int.MaxValue);
            parser.updateItems();

            List<DealabsItem> ItemsFiltres = parser.GetList("CDiscount");

            Assert.IsTrue(ItemsFiltres.Count == 2);
        }

        [Test]
        public void GetNouveauxDealsTest()
        {
            DealabsRssParser parser = new DealabsRssParser("flux1.xml", 2, int.MaxValue);
            parser.updateItems();

            List<DealabsItem> ListeNouveauxItems = parser.getNouveauxDeals(new DateTime(2014, 2, 13, 18 , 18 , 0 ));

            Assert.IsTrue(ListeNouveauxItems.Count == 8);
        }
    }
}
