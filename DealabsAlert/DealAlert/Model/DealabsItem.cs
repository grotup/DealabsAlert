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
        private HtmlDocument Document;

        public DealabsItem(string url, string titre, DateTime date, string Description)
        {
            this.UrlDealabs = url;
            this.titre = titre;
            this.date = date;
            this.description = Description;
            this.Document = GetHtmlDocument();
        }

        public override string ToString()
        {
            return titre;
        }

        /// <summary>
        /// Fonction qui renvoie l'URL de l'image associée au deal
        /// </summary>
        /// <returns>L'URL de l'image associée au deal</returns>
        public string ParserImage()
        {
            log.Debug("Entrée dans la méthode 'ParserImage'");

            HtmlNode NoeudLien = Document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            if (NoeudLien != null)
            {
                LinkImage = NoeudLien.GetAttributeValue("content", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserImage'. Valeur de sortie : " + LinkImage);
            return LinkImage;
        }

        /// <summary>
        /// Fonction qui renvoie l'URL du deal
        /// </summary>
        /// <returns>L'URL du deal</returns>
        public string ParserUrlDeal()
        {
            log.Debug("Entrée dans la méthode 'ParserUrlDeal'");

            HtmlNode NoeudLien = Document.DocumentNode.SelectSingleNode("//a[@class='voirledeal']");
            if (NoeudLien != null)
            {
                UrlDeal = NoeudLien.GetAttributeValue("href", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserUrlDeal'. Valeur de sortie : " + UrlDeal);
            return UrlDeal;
        }

        /// <summary>
        /// Fonction qui renvoie le code associé au deal
        /// </summary>
        /// <returns>Le code du deal</returns>
        public string ParserCode()
        {
            log.Debug("Entrée dans la méthode 'ParserCode'");

            HtmlNode NoeudLien = Document.DocumentNode.SelectSingleNode("//input[starts-with(@id,'voucher_code')]");
            if (NoeudLien != null)
            {
                Code = NoeudLien.GetAttributeValue("value", string.Empty);
            }
            log.Debug("Sortie de la méthode 'ParserCode'. Valeur de sortie : " + Code);
            return Code;
        }

        /// <summary>
        /// Fonction qui renvoie la "chaleur" du deal
        /// </summary>
        /// <returns>La "chaleur" du deal</returns>
        public string ParserDegre()
        {
            log.Debug("Entrée dans la méthode 'ParserDegre'");
            HtmlNode NoeudLien = Document.DocumentNode.SelectSingleNode("//div[@class='temperature_div']/p");
            //HtmlNode NoeudLien = Document.DocumentNode.SelectSingleNode("//div[starts-with(@id,'GetHotImage_color_')]");
            if (NoeudLien != null)
            {
                Degre = NoeudLien.InnerText;
            }
            log.Debug("Sortie de la méthode 'ParserDegre'. Valeur de sortie : " + Degre);
            return Degre;
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
