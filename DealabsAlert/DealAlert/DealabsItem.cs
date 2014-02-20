using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealabsAlert
{
    class DealabsItem
    {
        public string UrlDealabs;
        public string UrlDeal = string.Empty;
        public string Code = string.Empty;
        public string titre;
        public DateTime date;
        public string description;
        public string LinkImage = string.Empty;

        public DealabsItem(string url, string titre, DateTime date, string Description)
        {
            this.UrlDealabs = url;
            this.titre = titre;
            this.date = date;
            this.description = Description;
        }

        public override string ToString()
        {
            return titre;
        }

        public bool ParserImage()
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            if (NoeudLien != null)
            {
                LinkImage = NoeudLien.GetAttributeValue("content", string.Empty);
            }
            return !string.IsNullOrEmpty(LinkImage);
        }

        public bool ParserUrlDeal()
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//a[@class='voirledeal']");
            if (NoeudLien != null)
            {
                UrlDeal = NoeudLien.GetAttributeValue("href", string.Empty);
            }
            return !string.IsNullOrEmpty(UrlDeal);
        }

        public bool ParserCode()
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//input[starts-with(@id,'voucher_code')]");
            if (NoeudLien != null)
            {
                Code = NoeudLien.GetAttributeValue("value", string.Empty);
            }
            return !string.IsNullOrEmpty(Code);
        }
    }
}
