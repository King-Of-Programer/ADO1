using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly String ConnectionString = @"
            Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Alex\source\repos\ADO1\ADO1\Database1.mdf;Integrated Security=True";
    }
}
