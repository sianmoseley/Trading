using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TradingPlatform1.Models
{
    public class Trade
    {
        public Trade()
        {
            Status = "To be Processed";
            OrderDate = DateTime.Now;
            ProcessedDate = DateTime.Parse("1900-01-01");
        }

        public int TradeID { get; set; }
        public int ClientID { get; set; }
        public string TradeType { get; set; }
        public string Instrument { get; set; }
        public int Volume { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ProcessedDate { get; set; }


        public virtual Client Client { get; set; }
    }
}