﻿using System;

namespace DealabsParser.Model
{
    public class DealabsItem
    {
        public string UrlDealabs;
        public string UrlDeal = string.Empty;
        public string Code = string.Empty;
        public string titre;
        public DateTime date;
        public string description;
        public string LinkImage = string.Empty;
        public string Degre;

        public override string ToString()
        {
            return titre;
        }
    }
}
