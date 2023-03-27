using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.Entity
{
    public class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Department()
        {
            Id = Guid.NewGuid();
            Name = null;
            DeleteDt = null;
        }

        public Department(SqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Name = reader.GetString("Name");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
                ? null
                : reader.GetDateTime("DeleteDt");
        }

        public override string ToString()
        {
            return Id.ToString()[..5] + "..." + Name;
        }
    }
}
