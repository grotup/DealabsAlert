using System;
using System.Text;

using DealabsParser.Model;
using HtmlAgilityPack;

namespace DealabsParser.Parser
{
    public class DealabsItemParser
    {
        private string Url;
        private Parser NodeParser;
        private HtmlDocument Document;

        public DealabsItemParser(string Url)
        {
            this.Url = Url;
            this.Document = GetHtmlDocument();
            this.NodeParser = new Parser(this.Document);
        }

        public DealabsItem parserDeal(DealabsItem Item)
        {
            Item.LinkImage = ParserImage();
            Item.UrlDeal = ParserUrlDeal();
            Item.Code = ParserCode();
            Item.Degre = ParserDegre();

            return Item;
        }

        private string ParserImage()
        {
            return this.NodeParser.forNode("//meta[@property='og:image']").getAttribute("content");
        }

        /// <summary>
        /// Fonction qui renvoie l'URL du deal
        /// </summary>
        /// <returns>L'URL du deal</returns>
        private string ParserUrlDeal()
        {
            return NodeParser.forNode("//a[@class='voirledeal']").getAttribute("href");
        }

        /// <summary>
        /// Fonction qui renvoie le code associé au deal
        /// </summary>
        /// <returns>Le code du deal</returns>
        private string ParserCode()
        {
            return NodeParser.forNode("//input[(@class='voucher_code')]").getAttribute("value");
        }

        /// <summary>
        /// Fonction qui renvoie la "chaleur" du deal
        /// </summary>
        /// <returns>La "chaleur" du deal</returns>
        private string ParserDegre()
        {
            string value = NodeParser.forNode("//div[starts-with(@class,'temperature_div ')]/p").getInnerText();

            string[] Tab = value.Split(';');
            for (int i = 0; i < Tab.Length; i++)
            {
                if (Tab[i].Contains("°") || Tab[i] == "new")
                    return Tab[i];
            }
            return string.Empty;
        }

        /// <summary>
        /// Méthode qui charge un HTMLDocument à partir de l'URL de Dealabs
        /// </summary>
        /// <returns></returns>
        private HtmlDocument GetHtmlDocument()
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            HtmlWeb html = new HtmlWeb();
            document = html.Load(Url);
            return document;
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
                return NoeudLien != null ? NoeudLien.GetAttributeValue(attributeValue, string.Empty) : string.Empty;
            }

            public string getInnerText()
            {
                return NoeudLien != null ? NoeudLien.InnerText : string.Empty;
            }
        }
    }
}
