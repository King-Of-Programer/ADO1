using ADO1.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO1.Entity
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Surname { get; set; }
        public String Name { get; set; }
        public String Secname { get; set; }
        public Guid IdMainDep { get; set; } //Guid - ValueType, вживається для полів з модифікатором NOT NULL
        public Guid? IdSecDep { get; set; } // Якщо NULL не забронений, Guid використовується з Nullable(?)
        public Guid? IdChief { get; set; } //Gguid? - скорочення від Nullable<Guid>
        public DateTime? DeleteDt { get; set; }
        public Manager()
        {
            Id = Guid.NewGuid();
            Surname = null;
            Name = null;
            Secname = null;
            DeleteDt = null;
        }

        public Manager(SqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Surname = reader.GetString("Surname");
            Name = reader.GetString("Name");
            Secname = reader.GetString("Secname");
            IdMainDep = reader.GetGuid("Id_main_dep");

            IdSecDep = reader.GetValue("Id_sec_dep") == DBNull.Value
                    ? null
                    : reader.GetGuid("Id_sec_dep");

            IdChief = reader.IsDBNull("Id_chief")
                ? null
                : reader.GetGuid("Id_chief");

            DeleteDt = reader.IsDBNull("DeleteDt")
                ? null
                : reader.GetDateTime("DeleteDt");
        }

        public override string ToString()
        {
            return $"{Id.ToString()[..4]} {Surname} {Name} {Secname} {IdMainDep} {IdSecDep} {IdChief}";
        }

        //// NAVIGATION PROPERTIES ////

        internal DataContext? dataContext;

        public Entity.Department? MainDep
        {
            get
            {
                return dataContext?
                    .Departments
                    .GetAll()
                    .Find(d => d.Id == this.IdMainDep);
            }
        }
        public Entity.Department? SecDep
        {
            get
            {
                return dataContext?
                    .Departments
                    .GetAll()
                    .Find(d => d.Id == this.IdSecDep);
            }
        }
        public Entity.Manager? Chief
        {
            get
            {
                return dataContext?
                    .Managers
                    .GetAll()
                    .Find(m => m.Id == this.IdChief);
            }
        }
        public List<Entity.Manager>? Subordinates
        {
            get => dataContext?
                .Managers
                .GetAll()
                .Where(m => m.Id == this.IdChief)
                .ToList();
        }

    }
}
