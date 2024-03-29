﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.EfCore
{
    internal class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime? DeleteDt { get; set; }

        // NAVIGATION PROPERTIES /////////////////

        public List<Manager> Workers { get; set; }
        public List<Manager> SubWorkers { get; set; }
    }
}
