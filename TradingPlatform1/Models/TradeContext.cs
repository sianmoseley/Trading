using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TradingPlatform1.Models
{
    public class TradeContext : DbContext
    {

        public TradeContext() : base("ConString")
        {

        }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Client> Clients { get; set; }

    }
}