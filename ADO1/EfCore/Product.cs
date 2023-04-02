using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.EfCore
{
    internal class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }


        // NAVIGATION PROPERTIES /////////////////
       

        // колекція продажів(чеків)
        public List<Sale> Sales { get; set; }

        // колекція усіх продавців даного товару
        public List<Manager> Managers { get; set; }
    }
}
