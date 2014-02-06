using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DealabsAlert;
using System.Windows.Forms;

namespace DealAlert
{
    class ItemNotifier
    {
        private List<DealabsItem> listeItems;
        public static DealabsParser LeParser;

        public static void ThreadLoop()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                List<DealabsItem> ListeActuelle = LeParser.AlllistItems;
                LeParser.updateItems();

                if (LeParser.DateDernierItem > ListeActuelle.ElementAt(0).date)
                {
                    NotifyIcon notify = new NotifyIcon();
                    notify.BalloonTipText = "Coucou !";
                    
                    notify.ShowBalloonTip(10000);
                }
                Thread.Sleep(30000);
            }
        }
    }
}
