using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;

namespace DealAlert
{
    class DealabsItemParser
    {
        public string UrlPage;
        public string UrlDeal;

        public DealabsItemParser(string UrlPage)
        {
            this.UrlPage = UrlPage;
        }

        internal string GetUrlDealTest()
        {
            Stream stream = getStreamItemPage();
            StreamReader reader = new StreamReader(stream);
            string Buffer = reader.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.Load(Buffer);

            string UrlRet = doc.SelectSingleNode("//a[@class=\'voirledeal']").InnerText;

            return UrlRet;
        }

        private Stream getStreamItemPage()
        {
            WebClient wb = new WebClient();
            return wb.OpenRead(this.UrlPage);
        }
    }
}
