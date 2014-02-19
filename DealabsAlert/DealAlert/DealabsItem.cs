using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealabsAlert
{
    class DealabsItem
    {
        public string url;
        public string titre;
        public DateTime date;
        public string description;
        public string LinkImage = string.Empty;

        public DealabsItem(string url, string titre, DateTime date, string Description)
        {
            this.url = url;
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
            document = html.Load(url);

            document.DocumentNode.SelectSingleNode("//a[@class=voirledeal]");
            HtmlNode NoeudLien = document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            if (NoeudLien != null)
            {
                LinkImage = NoeudLien.GetAttributeValue("content", string.Empty);
            }
            return !string.IsNullOrEmpty(LinkImage);
        }
    }
}
