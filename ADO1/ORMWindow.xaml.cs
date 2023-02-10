using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for ORMWindow.xaml
    /// </summary>
    public partial class ORMWindow : Window
    {
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Product> Products { get; set; }


        private SqlConnection _connection;
        public ORMWindow()
        {
            InitializeComponent();
            Departments = new ObservableCollection<Entity.Department>();
            Products = new ObservableCollection<Entity.Product>();
           

            DataContext = this;

            _connection = new(App.ConnectionString);

          
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _connection.Open();
            LoadDepartments();
            LoadProducts();
        }

        #region SQL_COMMANDS

        private void ExecuteCommand(string command, string commandName)
        {
            SqlCommand cmd = new();
            cmd.Connection = _connection;
            cmd.CommandText = command;
            try//виконання команди
            {
                cmd.ExecuteNonQuery(); // NonQuery - без повернення результату
                MessageBox.Show(
                    commandName + " successfully complete",
                    commandName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose();
        }
        private void LoadDepartments()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT D.Id, D.Name FROM Departments D";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(new Entity.Department
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1)
                    });
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Window will be closed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LoadProducts()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT P.Id, P.Name, P.Price FROM Products P";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new Entity.Product
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDouble(2)
                    });
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Window will be closed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
            }
        }


        #endregion
        #region DOUBLE_CLICKS
        private void DepartmentsItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // робота з бд через ORM зведена до роботи з колекцією
            // sender - item, що містить посилання на Entity.Department у колекції Departments
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    //_dialogDepartment = new();
                    //_dialogDepartment.Department = department;
                    //if (_dialogDepartment.ShowDialog() == true)
                    //{
                    //    if (_dialogDepartment.Department is null) //Delete
                    //    {
                    //        string command =
                    //            "DELETE FROM Departments " +
                    //             $"WHERE Id = '{department.Id}'; ";
                    //        ExecuteCommand(command, $"Delete: {department.Name}");
                    //    }
                    //    else // Update
                    //    {
                    //        //MessageBox.Show(department.ToString());                            
                    //        string command =
                    //            "UPDATE Departments " +
                    //            $"SET Name = N'{department.Name}' " +
                    //            $"WHERE Id='{department.Id}';";
                    //        ExecuteCommand(command, "Update Department Name");
                    //        Departments.Clear();
                    //        LoadDepartments();
                    //    }
                   // }
                }
            }
        }

        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (sender is ListViewItem item)
            //{
            //    if (item.Content is Entity.Product product)
            //    {
            //        MessageBox.Show(product.ToString());
            //    }
            //}
        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                //if (item.Content is Entity.Manager manager)
                //{
                //    MessageBox.Show(manager.ToString());
                //}
            }
        }
        #endregion


    }
}
