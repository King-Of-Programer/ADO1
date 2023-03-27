using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.Entity
{
    public class Sale //ORM for table Sales
    {
        public Guid Id { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid ManagerId { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Sale()
        {
            Id = Guid.NewGuid();
            Quantity = 1;
            SaleDate = DateTime.Now;
        }

        public Sale(SqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            SaleDate = (DateTime)reader.GetValue("SaleDate");
            ProductId = reader.GetGuid("Product_Id");
            Quantity = reader.GetInt32("Quantity");
            ManagerId = reader.GetGuid("Manager_Id");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
                ? null
                : (DateTime)reader.GetValue("DeleteDt");
        }
    }
}
