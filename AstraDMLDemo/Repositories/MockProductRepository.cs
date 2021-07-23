using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Models;
using static AstraDMLDemo.Program;


namespace AstraDMLDemo.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        public MockProductRepository()
        {
        }

        public Product Add(Product product)
        {
            product.id = Guid.NewGuid();
            _productList.Add(product);
            return product;
        }

        public void Delete(Guid id)
        {
            Product product = _productList.FirstOrDefault(p => p.id == id);
            if (product != null)
            {
                _productList.Remove(product);
            }

            return;
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return _productList;
        }

        public Product GetProduct(Guid id)
        {
            return _productList.FirstOrDefault(p => p.id == id);
        }

        public bool IsRepositoryAvailable()
        {
            return true; 
        }

        public Product Update(Product productChanges)
        {
            Product product = _productList.FirstOrDefault(p => p.id == productChanges.id);
            if (product != null)
            {
                product.description = productChanges.description;
                product.price = productChanges.price;
                product.productname = productChanges.productname;
                product.created = productChanges.created;
                product.field1 = productChanges.field1;
                product.field2 = productChanges.field2;
                product.field3 = productChanges.field3;
                product.field4 = productChanges.field4;
                product.field5 = productChanges.field5;
                product.field6 = productChanges.field6;
                product.field7 = productChanges.field7;
                product.field8 = productChanges.field8;
                product.field9 = productChanges.field9;
                product.field10 = productChanges.field10;
            }

            return product;
        }
        public RepositoryDefinition RepositoryDefinition
        {
            get
            {
                return AstraDMLDemo.Program._repositoryDefinitions
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.Memory.ToString());
            }
        }
    }
}
