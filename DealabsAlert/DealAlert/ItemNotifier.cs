using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DealabsAlert;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace DealAlert
{
    class ItemNotifier
    {
        private List<DealabsItem> listeItems;
        public static DealabsParser LeParser;
        public Thread CallerThread;

        public static void ThreadLoop()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                ObservableCollection<DealabsItem> ListeActuelle = LeParser.AlllistItems;
                LeParser.updateItems(true);
                Thread.Sleep(30000);
            }
        }
    }
}
