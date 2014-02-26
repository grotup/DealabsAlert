using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealabsAlert
{
    class DealabsItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DealabsItem));

        public string UrlDealabs;
        public string UrlDeal = string.Empty;
        public string Code = string.Empty;
        public string titre;
        public DateTime date;
        public string description;
        public string LinkImage = string.Empty;
        public string Degre;

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

        public string ParserImage()
        {
            log.Debug("Entrée dans la méthode 'ParserImage'");
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            if (NoeudLien != null)
            {
                LinkImage = NoeudLien.GetAttributeValue("content", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserImage'. Valeur de sortie : " + LinkImage);
            return LinkImage;
        }

        public string ParserUrlDeal()
        {
            log.Debug("Entrée dans la méthode 'ParserUrlDeal'");
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//a[@class='voirledeal']");
            if (NoeudLien != null)
            {
                UrlDeal = NoeudLien.GetAttributeValue("href", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserUrlDeal'. Valeur de sortie : " + UrlDeal);
            return UrlDeal;
        }

        public string ParserCode()
        {
            log.Debug("Entrée dans la méthode 'ParserCode'");
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//input[starts-with(@id,'voucher_code')]");
            if (NoeudLien != null)
            {
                Code = NoeudLien.GetAttributeValue("value", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserCode'. Valeur de sortie : " + Code);
            return Code;
        }

        public string ParserDegre()
        {
            log.Debug("Entrée dans la méthode 'ParserDegre'");
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);

            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//div[starts-with(@id,'GetHotImage_color_')]");
            if (NoeudLien != null)
            {
                Degre = NoeudLien.InnerText;
            }
            log.Debug("Sortie de la méthode 'ParserDegre'. Valeur de sortie : " + Degre);
            return Degre;
        }
    }
}
