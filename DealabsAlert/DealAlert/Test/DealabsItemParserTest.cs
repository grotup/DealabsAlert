using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DealAlert;

namespace DealAlert.Test
{
    class DealabsItemParserTest
    {
        [Test]
        public void ContructorTest()
        {
            string UrlPage = "UrlPage1.htm";
            DealabsItemParser ItemParser = new DealabsItemParser(UrlPage);

            Assert.IsTrue(ItemParser.UrlPage == UrlPage);
        }

        [Test]
        public void GetUrlDealTest()
        {
            string UrlPage = "UrlPage1.htm";
            
            DealabsItemParser ItemParser = new DealabsItemParser(UrlPage);
            
            string UrlDeal = ItemParser.GetUrlDealTest();


            Assert.IsTrue(UrlDeal == @"http://www.dealabs.com/url/?e=6SxK67pSrYPwTbuTnBJs71GvNslAElQ8DuwneyIpus6VEmKge2vq5JaUh9Ua6ZGqIwfeZ13Xlww%3D");
        }
    }
}
