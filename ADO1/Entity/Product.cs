using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }
        public Product()
        {
            Id = Guid.NewGuid();
            Name = null;
            DeleteDt = null;
        }

        public Product(SqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Name = reader.GetString("Name");
            Price = reader.GetDouble("Price");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
            ? null
            : reader.GetDateTime(3);
        }

        public override string ToString()
        {
            return Id.ToString()[..5] + "... " + Name + " " + Price.ToString();
        }
    }
}
