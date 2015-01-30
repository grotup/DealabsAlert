using System;
using System.Text;

using DealabsParser.Model;
using HtmlAgilityPack;

namespace DealabsParser.Parser
{
    public class DealabsItemParser
    {
        /// <summary>
        /// Url de la page du deal
        /// </summary>
        private string Url;

        /// <summary>
        /// Instance de parser interne
        /// </summary>
        private Parser NodeParser;

        /// <summary>
        /// XmlDocument de la page du deal
        /// </summary>
        private HtmlDocument Document;

        private string _XPathImage = "//div[@id='floatant_title']/div[@class='structure']/div[@class='image_part']/div[@class='image_contener']/img";
        private string _AttributeImage = "src";

        private string _XPathUrlDeal = "//a[@class='voirledeal']";
        private string _AttributeUrlDeal = "href";

        private string _XPathCode = "//input[(@class='voucher_code')]";
        private string _AttributeCode = "value";

        private string _XPathTemperature = "//div[starts-with(@class,'temperature_div ')]/p";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Url"></param>
        public DealabsItemParser(string Url)
        {
            this.Url = Url;
            this.Document = GetHtmlDocument();
            this.NodeParser = new Parser(this.Document);
        }

        /// <summary>
        /// Méthode principale publique qui parse les élements
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        public DealabsItem parserDeal(DealabsItem Item)
        {
            Item.LinkImage = ParserImage();
            Item.UrlDeal = ParserUrlDeal();
            Item.Code = ParserCode();
            Item.Degre = ParserDegre();

            return Item;
        }

        /// <summary>
        /// Méthode qui parse l'URL de l'image du deal
        /// </summary>
        /// <returns></returns>
        private string ParserImage()
        {
            // //div[@id='floatant_title']/div[@class='structure]/div[@class='image_part']/div[@class='image_contener']/img
            return this.NodeParser
                .forNode(_XPathImage)
                .getAttribute(_AttributeImage);
        }

        /// <summary>
        /// Fonction qui renvoie l'URL du deal
        /// </summary>
        /// <returns>L'URL du deal</returns>
        private string ParserUrlDeal()
        {
            return this.NodeParser
                .forNode(_XPathUrlDeal)
                .getAttribute(_AttributeUrlDeal);
        }

        /// <summary>
        /// Fonction qui renvoie le code associé au deal
        /// </summary>
        /// <returns>Le code du deal</returns>
        private string ParserCode()
        {
            return NodeParser
                .forNode(_XPathCode)
                .getAttribute(_AttributeCode);
        }

        /// <summary>
        /// Fonction qui renvoie la "chaleur" du deal
        /// </summary>
        /// <returns>La "chaleur" du deal</returns>
        private string ParserDegre()
        {
            string value = NodeParser.forNode(_XPathTemperature).getInnerText();

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

        /// <summary>
        /// Internal classe servant de parser générique pour les élements du deal
        /// </summary>
        private class Parser
        {
            private HtmlNode NoeudLien;
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
