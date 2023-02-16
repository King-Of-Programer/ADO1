using ADO1.Entity;
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
        public ObservableCollection<Entity.Managers> Managers { get; set; }

        private SqlConnection _connection;
        private DepartmentCrudWindow _dialogDepartment;
        private NewDepartmentWindow _newDepartmentWindow;
        private ProductCudWindow _productDepartment;
        public ORMWindow()
        {
            InitializeComponent();

            Departments = new ObservableCollection<Entity.Department>();
            Products = new ObservableCollection<Entity.Product>();
            Managers = new ObservableCollection<Entity.Managers>();

            DataContext = this;

            _connection = new(App.ConnectionString);

            _dialogDepartment = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _connection.Open();
            LoadDepartments();
            LoadManagers();
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

        private void LoadManagers()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT M.Id, M.Surname, M.Name, M.Secname, M.Id_main_dep, M.Id_sec_dep, M.Id_chief  FROM Managers M";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(new Entity.Managers
                    {
                        Id = reader.GetGuid(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Secname = reader.GetString(3),
                        IdMainDep = reader.GetGuid(4),
                        IdSecDep = reader.GetValue(5) == DBNull.Value
                        ? null
                            : reader.GetGuid(5),
                        IdChief = reader.IsDBNull(6)
                        ? null
                            : reader.GetGuid(6)

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
                    _dialogDepartment = new();
                    _dialogDepartment.Department = department;
                    if (_dialogDepartment.ShowDialog() == true)
                    {
                        if (_dialogDepartment.Department is null) //Delete
                        {
                            string command =
                                "DELETE FROM Departments " +
                                 $"WHERE Id = @id; ";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", department.Id);
                            ExecuteCommand(cmd.ToString(), $"Delete: {department.Name}");
                            Departments.Clear();
                            LoadDepartments();
                        }
                        else // Update
                        {
                            //MessageBox.Show(department.ToString());                            
                            string command =
                                 "UPDATE Departments " +
                                 $"SET Name = @name " +
                                 $"WHERE Id=@id;";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", department.Id);
                            cmd.Parameters.AddWithValue("@name", department.Name);
                            ExecuteCommand(cmd.ToString(), "Update Department Name");
                            Departments.Clear();
                            LoadDepartments();
                        }
                    }
                }
            }
        }

        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Product product)
                {
                    _productDepartment = new();
                    _productDepartment.Product = product;
                    if (_productDepartment.ShowDialog() == true)
                    {
                        if (_productDepartment.Product is null) //Delete
                        {
                            string command =
                                "DELETE FROM Products " +
                                 $"WHERE Id = '{product.Id}'; ";
                            ExecuteCommand(command, $"Delete: {product.Name}");
                            Departments.Clear();
                            LoadProducts();
                        }
                        else // Update
                        {
                            //MessageBox.Show(department.ToString());                            
                            string command =
                                "UPDATE Products " +
                                $"SET Name = N'{product.Name}' " +
                                $"WHERE Id='{product.Id}';";
                            ExecuteCommand(command, "Update Proucts Name");
                            Departments.Clear();
                            LoadProducts();
                        }
                    }
                }
            }

        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Managers manager)
                {
                    MessageBox.Show(manager.ToString());
                }
            }

        }
        #endregion

        private void newDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            Entity.Department department = new();
            _newDepartmentWindow = new NewDepartmentWindow();
            _newDepartmentWindow.Department = department;
            if (_newDepartmentWindow.ShowDialog() == true)
            {
                if (department != null)
                {

                    String sql = "INSERT INTO Department(Id, Name)" +
                     " VALUES(@id, @name)";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", department.Id);
                    cmd.Parameters.AddWithValue("@name", department.Name);
                    ExecuteCommand(cmd.ToString(), "Add new Product");
                    Departments.Clear();
                    LoadDepartments();
                }
            }
           
        }

        private void newProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductCudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Product is not null)
                {
                    //НЕ РЕКОМЕНДУЄТЬСЯ 
                    //String sql = $"INSERT INTO Products(Id, Name, Price)" +
                    //    $" VALUES('{dialog.Product.Id}', N'{dialog.Product.Name}', {dialog.Product.Price})";
                    //ExecuteCommand(sql, "Add new Product");
                    //Products.Clear();
                    //LoadProducts();
                    //MessageBox.Show(dialog.Product.ToString());

                    String sql = "INSERT INTO Products(Id, Name, Price)" +
                     " VALUES(@id, @name, @price)";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Product.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.Product.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.Product.Price);
                    ExecuteCommand(cmd.ToString(), "Add new Product");
                    Products.Clear();
                    LoadProducts();
                }
            }

        }



    }


}

