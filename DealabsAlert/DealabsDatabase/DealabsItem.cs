using SQLite;
using System;
using System.Collections.Generic;

namespace DealabsDatabase
{
    public class DealabsItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UrlDealabs;
        public string UrlDeal = string.Empty;
        public string Code = string.Empty;
        public string titre {get ; set;}
        public DateTime date;
        public string description;
        public string LinkImage = string.Empty;
        public string Degre { get; set; }

        public override string ToString()
        {
            return titre;
        }
    }
}
