using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealabsAlert
{
    public class DealabsItem
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
        private HtmlDocument Document;
        private Parser NodeParser;

        public DealabsItem(string url, string titre, DateTime date, string Description)
        {
            this.UrlDealabs = url;
            this.titre = titre;
            this.date = date;
            this.description = Description;
            this.Document = GetHtmlDocument();
            this.NodeParser = new Parser(this.Document);
        }

        public override string ToString()
        {
            return titre;
        }

        private class Parser
        {
            private HtmlNode NoeudLien;
            private string Link;
            private HtmlDocument Document;

            internal Parser(HtmlDocument Document)
            {
                this.Document = Document;
            }

            public Parser forNode(string node)
            {
                NoeudLien = Document.DocumentNode.SelectSingleNode(node);
                return this;
            }

            public string getAttribute(string attributeValue)
            {
                if (NoeudLien != null)
                {
                    Link = NoeudLien.GetAttributeValue(attributeValue, string.Empty);
                }
                //LOG ?
                return Link;
            }

            public string getInnerText()
            {
                if (NoeudLien != null)
                {
                    Link = NoeudLien.InnerText;
                }
                //LOG ?
                return Link;
            }
        }

        /// <summary>
        /// Fonction qui renvoie l'URL de l'image associée au deal
        /// </summary>
        /// <returns>L'URL de l'image associée au deal</returns>
        public string ParserImage()
        {
            return this.LinkImage = NodeParser.forNode("//meta[@property='og:image']").getAttribute("content");
        }

        /// <summary>
        /// Fonction qui renvoie l'URL du deal
        /// </summary>
        /// <returns>L'URL du deal</returns>
        public string ParserUrlDeal()
        {
            return this.UrlDeal = NodeParser.forNode("//a[@class='voirledeal']").getAttribute("href");
        }

        /// <summary>
        /// Fonction qui renvoie le code associé au deal
        /// </summary>
        /// <returns>Le code du deal</returns>
        public string ParserCode()
        {
            return this.Code = NodeParser.forNode("//input[starts-with(@id,'voucher_code')]").getAttribute("value");
        }

        /// <summary>
        /// Fonction qui renvoie la "chaleur" du deal
        /// </summary>
        /// <returns>La "chaleur" du deal</returns>
        public string ParserDegre()
        {
            return this.Degre = NodeParser.forNode("//div[@class='temperature_div']/p").getInnerText();
        }

        /// <summary>
        /// Méthode qui charge un HTMLDocument à partir de l'URL de Dealabs
        /// </summary>
        /// <returns></returns>
        private HtmlDocument GetHtmlDocument()
        {
            log.Debug("Génération d'un HTMLDocument");
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(UrlDealabs);
            return document;
        }
    }
}
