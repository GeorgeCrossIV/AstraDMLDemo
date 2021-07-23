using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Enums
{
    /// <summary>
    /// The method used to connect the client to the data source (API, Memory, Driver)
    /// </summary>
    public enum ConnectionType
    {
        [Description("API")]
        API,

        [Description("Memory")]
        Memory,

        [Description("Driver")]
        Driver

    }
}
