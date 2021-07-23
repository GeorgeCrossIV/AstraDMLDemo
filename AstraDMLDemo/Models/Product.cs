using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Models
{
    public class Product
    {
        public Product()
        {
        }
        [Key]
        [JsonProperty(PropertyName = "id")]
        [Display(Name = "Id")]
        public Guid id { get; set; }

        [JsonProperty(PropertyName = "productname")]
        [Display(Name = "Product Name")]
        public string productname { get; set; }

        [JsonProperty(PropertyName = "description")]
        [Display(Name = "Description")]
        public string  description { get; set; }

        [JsonProperty(PropertyName = "price")]
        [Display(Name = "Price")]
        public decimal price { get; set; }

        [JsonProperty(PropertyName = "created")]
        [Display(Name = "Date Created")]
        public DateTime created { get; set; }

        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
        public string field6 { get; set; }
        public string field7 { get; set; }
        public string field8 { get; set; }
        public string field9 { get; set; }
        public string field10 { get; set; }

        public string GetRestColumns()
        {
            RestColumns restColumns = new RestColumns();

            restColumns.Columns.Add(new RestColumn { Name = "id", Value = this.id.ToString() });
            restColumns.Columns.Add(new RestColumn { Name = "productname", Value = this.productname.ToString() });
            restColumns.Columns.Add(new RestColumn { Name = "description", Value = this.description.ToString() });
            restColumns.Columns.Add(new RestColumn { Name = "price", Value = this.price.ToString() });
            restColumns.Columns.Add(new RestColumn { Name = "created", Value = this.created.ToString("u").Replace(' ','T') });

            return JsonConvert.SerializeObject(restColumns);
        }

        public string GetGraphQlInsertJson()
        {
            string mutation = string.Format(
                "mutation {{" +
                "newrecord: insertproducts(" +
                "value: {{ id: \\\"{0}\\\" productname: \\\"{1}\\\", description: \\\"{2}\\\", price: \\\"{3}\\\", created: \\\"{4}\\\" }}" + 
                "options: {{consistency: LOCAL_QUORUM }}" +
                ") {{"+
                "value {{"+
                "id " +
                "}}" +
                "}}"+
                "}}"
                ,id.ToString(),
                productname.ToString(),
                description.ToString(), 
                price.ToString(), 
                created.ToString("u").Replace(' ', 'T'));

            string query = string.Format("{{\"query\": \"{0}\", \"variables\":{{}}}}", mutation);

            return query;
        }

        internal Product GetCorrectedTimestamp()
        {
            this.created = DateTime.SpecifyKind(this.created, DateTimeKind.Utc);

            return this;
        }
    }


}
