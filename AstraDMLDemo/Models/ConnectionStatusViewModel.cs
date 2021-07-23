using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Models
{
    /// <summary>
    /// Model View for displaying connection information
    /// </summary>
    public class ConnectionStatusItem
    {
        public string Name { get; set; }
        public string ServerType { get; set; }
        public bool Available { get; set; }
    }
}
