using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Enums
{
    /// <summary>
    /// Describes the repository ([Astra | DSE] [CQL | Rest | GraphQL | Document])
    /// </summary>
    public enum RepositoryType
    {
        [Description("Astra CQL")]
        AstraCql,

        [Description("DSE CQL")]
        DseCql,

        [Description("Astra Rest API")]
        Rest,

        [Description("Astra GraphQL API")]
        GraphQl,

        [Description("Memory")]
        Memory,

        [Description("Astra Document API")]
        Document,

        [Description("DSE Rest API")]
        DseRest,

        [Description("DSE GraphQL API")]
        DseGraphQl,

        [Description("DSE Document API")]
        DseDocument

    }
}