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

        public DealabsItem(string url, string titre, DateTime date)
        {
            this.url = url;
            this.titre = titre;
            this.date = date;
        }

        public override string ToString()
        {
            return titre;
        }
    }
}
