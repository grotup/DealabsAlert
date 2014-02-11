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
        public ObservableCollection<DealabsItem> AlllistItems;
        public ObservableCollection<DealabsItem> listItemsFiltres;
        public ObservableCollection<DealabsItem> listItemsAffichee;
        public DateTime DateDernierItem;
        private bool premierUpdate = true;

        public DealabsParser(string url, int nbMinutes)
        {
            this.url = url;
            this.nbMinutes = nbMinutes;
        }

        public ObservableCollection<DealabsItem> filtrerItems(string filtre)
        {
            // Si on demande de filtrer alors qu'on n'a pas de liste, on la crée
            if (AlllistItems == null)
            {
                updateItems(false);
            }
            ObservableCollection<DealabsItem> retList = new ObservableCollection<DealabsItem>();
            foreach (DealabsItem item in AlllistItems)
            {
                if (item.titre.ToUpper().Contains(filtre.ToUpper()))
                {
                     retList.Add(item);
                }
            }
            this.listItemsFiltres = retList;
            listItemsAffichee = listItemsFiltres;
            return retList;
        }

        private Stream getStreamRSS()
        {
            WebClient wb = new WebClient();
            return wb.OpenRead(this.url);
        }

        private void saveItems()
        {
            StreamWriter streamItems = new StreamWriter("liste");
            foreach( DealabsItem item in AlllistItems )
            {
                streamItems.Write(item.date.ToString() + "///" + item.titre + "///" + item.url + "\n");
            }
        }

        internal void updateItems(bool notifier)
        {
            ObservableCollection<DealabsItem> retList = new ObservableCollection<DealabsItem>();
            Stream stream = getStreamRSS();

            // On charge le stream en Xml
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNodeList listItems = doc.GetElementsByTagName("item");

            // On renvoie que les items qu'on veut dans le app.config
            foreach (XmlNode item in listItems)
            {
                string date = item.SelectSingleNode("pubDate").InnerText;
                DateTime DateFormatted = Convert.ToDateTime(date);
                if ((DateFormatted < (DateTime.Now.AddMinutes(-nbMinutes))) == false)
                {
                    retList.Add(new DealabsItem(item.SelectSingleNode("link").InnerText, item.SelectSingleNode("title").InnerText, DateFormatted));
                }
            }
            AlllistItems = retList;
            listItemsAffichee = AlllistItems;
            if ((DateDernierItem != AlllistItems.ElementAt(0).date) && (!premierUpdate) && (notifier))
            {
                listChanged();
            }
            if (premierUpdate) premierUpdate = false;
            this.DateDernierItem = AlllistItems.ElementAt(0).date;
        }

        internal int getNbItems()
        {
            return (AlllistItems == null) ? 0 : AlllistItems.Count;
        }

        internal void resetFiltre()
        {
            this.listItemsAffichee = AlllistItems;
        }

        public void listChanged()
        {
            // Pour chaque item, on regarde la date
            DateTime DateItem = AlllistItems.ElementAt(0).date;
            int NbNouveauxItems = 0;
            while (DateItem != this.DateDernierItem)
            {
                DateItem = AlllistItems.ElementAt(NbNouveauxItems).date;
                NbNouveauxItems++;
            }
            NotificationWindow window = new NotificationWindow("Dealabs : " + NbNouveauxItems + " nouveaux deals !");
        }
    }
}
