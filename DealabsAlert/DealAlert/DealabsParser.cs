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
using DealAlert;

namespace DealabsAlert
{
    class DealabsParser
    {
        private string url;
        private int nbMinutes;
        public List<DealabsItem> AlllistItems = new List<DealabsItem>();
        public List<DealabsItem> listItemsFiltres;
        public List<DealabsItem> listItemsAffichee;
        public DateTime DateDernierItem;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="url">URL de Dealabs</param>
        /// <param name="nbMinutes"></param>
        public DealabsParser(string url, int nbMinutes)
        {
            this.url = url;
            this.nbMinutes = nbMinutes;
        }

        public List<DealabsItem> filtrerItems(string filtre)
        {
            // Si on demande de filtrer alors qu'on n'a pas de liste, on la crée
            if (AlllistItems == null)
            {
                updateItems(false);
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
            this.listItemsFiltres = retList;
            return retList;
        }

        internal void updateItems(bool notifier)
        {
            List<DealabsItem> retList = new List<DealabsItem>();
            // On ouvre un stream
            Stream stream = getStreamRSS();

            // On charge le stream en Xml
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNodeList listItems = doc.GetElementsByTagName("item");

            // Pour chaque item
            foreach (XmlNode item in listItems)
            {
                // On crée un objet qu'on ajoute dans la liste
                string date = item.SelectSingleNode("pubDate").InnerText;
                DateTime DateFormatted = Convert.ToDateTime(date);
                if ((DateFormatted < (DateTime.Now.AddMinutes(-nbMinutes))) == false)
                {
                    retList.Add(new DealabsItem(item.SelectSingleNode("link").InnerText, item.SelectSingleNode("title").InnerText, DateFormatted));
                }
            }
            // On définit le dernier item daté
            this.AlllistItems = retList;
            this.listItemsAffichee = retList;
            this.DateDernierItem = AlllistItems.ElementAt(0).date;
        }

        private Stream getStreamRSS()
        {
            WebClient wb = new WebClient();
            return wb.OpenRead(this.url);
        }

        public List<DealabsItem> getNouveauxDeals(DateTime DateFrom)
        {
            List<DealabsItem> ret = new List<DealabsItem>();
            int idxDeals = 0;
            bool sortir = false;
            // Tant qu'on n'a pas trouvé de nouveaux deals
            while (!sortir)
            {
                // Si la date est anterieure à celle passée en paramètre, on ajoute le deal au retour
                if (AlllistItems.ElementAt(idxDeals).date.CompareTo(DateFrom) > 0)
                {
                    ret.Add(AlllistItems.ElementAt(idxDeals));
                }else
                {
                    // Dès qu'on a une date équivalente ou inferieure, c'est qu'on n'a plus de nouveaux items
                    sortir = true;
                }
                idxDeals++;
            }
            // Et on renvoie la liste des deals
            return ret;
        }

        internal void resetFiltre()
        {
            // On définit comme étant affiché tous les deals
            this.listItemsAffichee = AlllistItems;
        }
    }
}
