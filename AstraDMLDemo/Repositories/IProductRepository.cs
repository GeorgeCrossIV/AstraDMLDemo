using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Models;

namespace AstraDMLDemo.Repositories
{
    public interface IProductRepository
    {        
        Product GetProduct(Guid id);
        IEnumerable<Product> GetAllProduct();
        Product Add(Product product);
        void Delete(Guid id);
        Product Update(Product product);
        bool IsRepositoryAvailable();
        RepositoryDefinition RepositoryDefinition { get; }
   }
}
