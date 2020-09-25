using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TradingPlatform1.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        ICollection<Trade> Trades { get; set; }

      
    }
}