using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using log4net;
using DealabsParser.Model;

namespace DealabsParser.Parser
{
    public class DealabsRssParser
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DealabsRssParser));

        private string url;
        private int nbMinutes;
        private int nb_items_parsing;
        private List<DealabsItem> AlllistItems = new List<DealabsItem>();
        public DateTime DateDernierItem;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="url">URL de Dealabs</param>
        /// <param name="nbMinutes"></param>
        public DealabsRssParser(string url, int nbMinutes, int nb_items_parsing)
        {
            this.url = url;
            this.nbMinutes = nbMinutes;
            this.nb_items_parsing = nb_items_parsing;
        }

        /// <summary>
        /// Fonction qui rafraichit la liste principale des deals
        /// </summary>
        public void updateItems()
        {
            log.Debug("Entrée dans la méthode 'updateItem'");
            long TickEntree = DateTime.Now.Ticks;
            List<DealabsItem> retList = new List<DealabsItem>();
            // On ouvre un stream
            Stream stream = getStreamRSS();

            // On charge le stream en Xml
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNodeList listItems = doc.GetElementsByTagName("item");

            // On se limite en nombre de deals
            int nbItemsMax = nb_items_parsing;
            // Pour chaque item
            foreach (XmlNode item in listItems)
            {
                // On crée un objet qu'on ajoute dans la liste
                string date = item.SelectSingleNode("pubDate").InnerText;
                DateTime DateFormatted = Convert.ToDateTime(date);
                if (DateFormatted.CompareTo(DateDernierItem) > 0)
                {
                    DealabsItem ItemToAdd = new DealabsItem();
                    ItemToAdd.UrlDealabs = item.SelectSingleNode("link").InnerText;
                    ItemToAdd.titre = item.SelectSingleNode("title").InnerText;
                    ItemToAdd.date = DateFormatted;
                    ItemToAdd.description = item.SelectSingleNode("description").InnerText;
                    ItemToAdd.Degre = "NC";
                    DealabsItemParser ItemParser = new DealabsItemParser(ItemToAdd.UrlDealabs);
                    ItemToAdd = ItemParser.parserDeal(ItemToAdd);
                    retList.Add(ItemToAdd);
                }
                else
                {
                    break;
                }
                // On décrémente, et si on est à 0 on break;
                nbItemsMax--;
                // Si on a parsé 100 items, on arrête.
                if (nbItemsMax == 0)
                {
                    break;
                }
            }
            long Duree = DateTime.Now.Ticks - TickEntree;
            log.Debug("Durée de l'update = " + new TimeSpan(Duree).TotalMilliseconds + "ms");

            // On définit le dernier item daté
            MergerListePrincipale(retList);
            this.DateDernierItem = AlllistItems.ElementAt(0).date;
            
        }

        private void MergerListePrincipale(List<DealabsItem> tmp)
        {
            for (int i = tmp.Count - 1; i >= 0; i--)
            {
                this.AlllistItems.Insert(0, tmp.ElementAt(i));
            }
            this.AlllistItems = AlllistItems.OrderByDescending(x => x.date).ToList();
        }

        /// <summary>
        /// Fonction qui lance le parsing des items contenus dans la liste
        /// </summary>
        /// <returns>La liste des items</returns>
        public List<DealabsItem> ParserDealItems()
        {
            for (int i = 0; i < AlllistItems.Count; i++)
            {
                DealabsItemParser Parser = new DealabsItemParser(AlllistItems[i].UrlDealabs);
                AlllistItems[i] = Parser.parserDeal(AlllistItems[i]);
            }

            return AlllistItems;
        }

        /// <summary>
        /// Fonction qui renvoie les nouveaux deals par rapport à une date passée en paramètre
        /// </summary>
        /// <param name="DateFrom">La date depuis laquelle on veut les nouveaux items</param>
        /// <returns>La liste des nouveaux items depuis la date DateFrom</returns>
        public List<DealabsItem> getNouveauxDeals(DateTime DateFrom)
        {
            List<DealabsItem> ret = new List<DealabsItem>();
            int idxDeals = 0;
            bool sortir = false;
            // Tant qu'on n'a pas trouvé de nouveaux deals
            while (!sortir)
            {
                // Si la date est anterieure à celle passée en paramètre, on ajoute le deal au retour
                if (idxDeals < AlllistItems.Count && AlllistItems.ElementAt(idxDeals).date.CompareTo(DateFrom) > 0)
                {
                    ret.Add(AlllistItems.ElementAt(idxDeals));
                }
                else
                {
                    // Dès qu'on a une date équivalente ou inferieure, c'est qu'on n'a plus de nouveaux items
                    sortir = true;
                }
                idxDeals++;
            }
            // Et on renvoie la liste des deals
            return ret;
        }

        /// <summary>
        /// Fonction qui renvoie la liste des items, selon un filtre. Si le filtre est string.empty, on renvoie la liste de tous les deals,
        /// sinon, on renvoie la liste des items filtrés
        /// </summary>
        /// <param name="p">Filtre pour la liste, "" pour avoir toute la liste</param>
        /// <returns>La liste des items</returns>
        public List<DealabsItem> GetList(string p)
        {
            // Si on n'a pas de filtre, on renvoie directement la liste
            if (string.IsNullOrEmpty(p))
            {
                return AlllistItems;
            }
            // Si on en a un, on renvoie une liste filtrée
            return this.filtrerItems(p);
        }

        /// <summary>
        /// Fonction qui filtre les items de la liste principale, et qui renvoie une liste de ces items
        /// </summary>
        /// <param name="filtre">Le filtre correspondant</param>
        /// <returns>Liste des items filtrés</returns>
        private List<DealabsItem> filtrerItems(string filtre)
        {
            // Si on demande de filtrer alors qu'on n'a pas de liste, on la crée
            if (AlllistItems == null)
            {
                updateItems();
            }
            List<DealabsItem> retList = new List<DealabsItem>();
            // Pour chaque item ...
            foreach (DealabsItem item in AlllistItems)
            {
                // Si le titre contient ce qu'on cherche
                if (item.titre.ToUpper().Contains(filtre.ToUpper()))
                {
                    retList.Add(item);
                }
            }
            return retList;
        }

        /// <summary>
        /// Fonction qu irenvoie le stream correspondant à l'URL
        /// </summary>
        /// <returns>Stream créé</returns>
        private Stream getStreamRSS()
        {
            WebClient wb = new WebClient();
            return wb.OpenRead(this.url);
        }

        /// <summary>
        /// Méthode qui renvoie p items qui sont publiés avant la date passée en paramètre
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="p"></param>
        public void ChargerItemAvant(DateTime dateTime, int p)
        {
            List<DealabsItem> retList = new List<DealabsItem>();
            Stream RssStream = getStreamRSS();

            // On charge le stream en Xml
            XmlDocument doc = new XmlDocument();
            doc.Load(RssStream);
            XmlNodeList listItems = doc.GetElementsByTagName("item");

            // On se limite en nombre de deals
            int nbItemsMax = p;
            // Pour chaque item
            foreach (XmlNode item in listItems)
            {
                // On crée un objet qu'on ajoute dans la liste
                string date = item.SelectSingleNode("pubDate").InnerText;
                DateTime DateFormatted = Convert.ToDateTime(date);
                if (DateFormatted.CompareTo(dateTime) < 0)
                {
                    DealabsItem ItemToAdd = new DealabsItem();
                    ItemToAdd.UrlDealabs = item.SelectSingleNode("link").InnerText;
                    ItemToAdd.titre = item.SelectSingleNode("title").InnerText;
                    ItemToAdd.date = DateFormatted;
                    ItemToAdd.description = item.SelectSingleNode("description").InnerText;
                    ItemToAdd.Degre = "NC";
                    DealabsItemParser ItemParser = new DealabsItemParser(ItemToAdd.UrlDealabs);
                    ItemToAdd = ItemParser.parserDeal(ItemToAdd);
                    retList.Add(ItemToAdd);
                    nbItemsMax--;
                }
                // Si on a parsé 100 items, on arrête.
                if (nbItemsMax == 0)
                {
                    break;
                }
            }

            // On définit le dernier item daté
            MergerListePrincipale(retList);
            this.DateDernierItem = AlllistItems.ElementAt(0).date;
        }
    }
}
