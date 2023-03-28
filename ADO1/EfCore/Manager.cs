using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.EFCore
{
    public class Manager
    {
        public Guid Id { get; set; }
        [Required]
        public String Surname { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Secname { get; set; }
        public Guid IdMainDep { get; set; } 
        public Guid? IdSecDep { get; set; } 
        public Guid? IdChief { get; set; } 
        public DateTime? DeleteDt { get; set; }
    }
}
