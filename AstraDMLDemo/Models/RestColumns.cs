using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AstraDMLDemo.Models
{
    public class RestColumns
    {
        public RestColumns()
        {
            this.Columns = new List<RestColumn>();
        }
        [JsonProperty(PropertyName = "columns")]

        public List<RestColumn> Columns { get; set; }
    }

    public class RestColumn
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }



    //public List<RestColumn> GetColumns(Product product)
    //{
    //    List<RestColumn> columns = new List<RestColumn>();

    //    PropertyInfo[] propertyInfos;
    //    propertyInfos = typeof(Product).GetProperties(BindingFlags.Public | BindingFlags.Static);
    //    Array.Sort(propertyInfos, delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
    //     { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

    //    foreach (PropertyInfo propertyInfo in propertyInfos)
    //    {
    //        columns.Add(new RestColumn
    //        {
    //            Name = propertyInfo.Name,
    //            Value = "test"
    //        });
    //    }

    //    return columns;
    //}
}
