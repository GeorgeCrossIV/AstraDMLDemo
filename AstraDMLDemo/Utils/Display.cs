using AstraDMLDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Utils
{
    public class Display
    {
        private static DataTable _ProductTable;

        private static void CreateProductTable()
        {
            DataColumn column;

            _ProductTable = new DataTable("Product");

            column = new DataColumn("id", System.Type.GetType("System.String"));
            _ProductTable.Columns.Add(column);
            column = new DataColumn("description", System.Type.GetType("System.String"));
            _ProductTable.Columns.Add(column);
            column = new DataColumn("price", System.Type.GetType("System.Decimal"));
            _ProductTable.Columns.Add(column);
            column = new DataColumn("productname", System.Type.GetType("System.String"));
            _ProductTable.Columns.Add(column);
            column = new DataColumn("created", System.Type.GetType("System.DateTime"));
            _ProductTable.Columns.Add(column);
        }

        public static DataTable ProductTable(IEnumerable<Product> products)
        {
            DataRow row;

            CreateProductTable();
            foreach (Product product in products)
            {
                row = _ProductTable.NewRow();
                row["id"] = product.id;
                row["description"] = product.description;
                row["price"] = product.price;
                row["productname"] = product.productname;
                row["created"] = product.created;
                _ProductTable.Rows.Add(row);
            }

            return _ProductTable;
        }
    }
}
