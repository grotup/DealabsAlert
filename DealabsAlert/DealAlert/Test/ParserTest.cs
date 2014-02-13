using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DealabsAlert;
using System.IO;

namespace DealAlert.Test
{
    class ParserTest
    {
        [Test]
        public void parserRSSTest()
        {
            DealabsParser parser = new DealabsParser("flux1.xml", 2);
            parser.updateItems();

            Assert.IsTrue(parser.GetList(string.Empty).Count == 27);
        }

        [Test]
        public void GetListSansFiltre()
        {
            DealabsParser parser = new DealabsParser("flux1.xml", 2);
            parser.updateItems();

            List<DealabsItem> ItemsFiltres = parser.GetList("");

            Assert.IsTrue(ItemsFiltres.Count == 27);
        }

        [Test]
        public void GetListAvecFiltre()
        {
            DealabsParser parser = new DealabsParser("flux1.xml", 2);
            parser.updateItems();

            List<DealabsItem> ItemsFiltres = parser.GetList("CDiscount");

            Assert.IsTrue(ItemsFiltres.Count == 2);
        }

        [Test]
        public void GetNouveauxDealsTest()
        {
            DealabsParser parser = new DealabsParser("flux1.xml", 2);
            parser.updateItems();

            List<DealabsItem> ListeNouveauxItems = parser.getNouveauxDeals(new DateTime(2014, 2, 13, 18 , 18 , 0 ));

            Assert.IsTrue(ListeNouveauxItems.Count == 8);
        }
    }
}
