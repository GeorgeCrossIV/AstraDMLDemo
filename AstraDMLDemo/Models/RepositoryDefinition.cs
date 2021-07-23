using AstraDMLDemo.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Models
{
    public class RepositoryDefinition
    {
        [Key]
        public string Name { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public InterfaceType InterfaceType { get; set; }
        public string DatabaseName { get; set; }
        public string Keyspace { get; set; }
        public string Address { get; set; }
        public string UserClientId { get; set; }
        public string PasswordSecret { get; set; }
        public string Token { get; set; }
        public string Region { get; set; }
        public string TableName { get; set; }
        public string Server { get; set; }
    }
}
