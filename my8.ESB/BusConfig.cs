using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB
{
    public class BusConfig
    {
        public BusConfig() { }

        public string Host { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public BusConfigHandler[] Handlers { get; set; }
    }
    public class BusConfigHandler
    {
        public BusConfigHandler() { }

        public int Concurency { get; set; }
        public string Queue { get; set; }
    }
}
