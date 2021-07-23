using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Enums
{
    /// <summary>
    /// The Query method. Defines which language is used to execute data manipulation
    /// </summary>
    public enum InterfaceType
    {
        [Description("Cassandra C# Driver")]
        CassandraCSharpDriver,

        [Description("Document API")]
        DocumentApi,

        [Description("GraphQL API")]
        GraphQlApi,

        [Description("Rest API")]
        RestApi,

        [Description("Memory")]
        Memory

    }
}
