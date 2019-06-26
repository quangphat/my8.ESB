using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace my8.ESB
{
    [DataContract]
    public class DoSomething
    {
        /// <summary>
        /// The Thing
        /// </summary>
        public DoSomething() { }
        [DataMember]
        public string Value { get; set; }
    }
}
