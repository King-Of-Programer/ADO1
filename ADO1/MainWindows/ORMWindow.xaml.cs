using ADO1.CRUDWindows;
using ADO1.Entity;
using ADO1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
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

    public partial class ORMWindow : Window
    {
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Product> Products { get; set; }
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public ObservableCollection<Entity.Sale> Sales { get; set; }

        private SqlConnection _connection;

        private DepartmentCrudWindow _dialogDepartment;
        private ProductCrudWindow _dialogProduct;


        public ORMWindow()
        {
            InitializeComponent();

            Departments = new ObservableCollection<Entity.Department>();
            Products = new ObservableCollection<Entity.Product>();
            Managers = new ObservableCollection<Entity.Manager>();
            Sales = new ObservableCollection<Sale>();

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
            LoadSales();
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
        private void ExecuteCommand(SqlCommand command, string commandName)
        {
            try//виконання команди
            {
                command.ExecuteNonQuery(); // NonQuery - без повернення результату
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
            command.Dispose();
        }

        #endregion

        #region LOAD_DATA
        //ЗАВАНТАЖИТИ ВІДДІЛИ
        private void LoadDepartments()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT * FROM Departments D WHERE D.DeleteDt IS NULL";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(new Entity.Department(reader));
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
        //ЗАВАНТАЖИТИ ТОВАРИ
        private void LoadProducts()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT * FROM Products P WHERE P.DeleteDt IS NULL";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new Entity.Product(reader));
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

        //ЗАВАНТАЖИТИ КЕРІВНИКІВ
        private void LoadManagers()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT * FROM Managers M WHERE M.DeleteDt IS NULL";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(new Entity.Manager(reader));
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

        //ЗАВАНТАЖИТИ ПРОДАЖІ
        private void LoadSales()
        {
            SqlCommand cmd = new() { Connection = _connection };
            try
            {
                cmd.CommandText = "SELECT S.* FROM Sales S WHERE S.DeleteDt IS NULL";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sales.Add(new Entity.Sale(reader));
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
                               @"UPDATE Departments
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE Id = @id; ";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", department.Id);
                            ExecuteCommand(cmd, $"Delete: {department.Name}");
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
                            ExecuteCommand(cmd, "Update Department Name");
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
                    //MessageBox.Show(product.ToString());
                    _dialogProduct = new();
                    _dialogProduct.Product = product;
                    if (_dialogProduct.ShowDialog() == true)
                    {
                        if (_dialogProduct.Product is null) //Delete
                        {
                            string command =
                                 @"UPDATE Products
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE Id = @id; ";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", product.Id);
                            ExecuteCommand(cmd, $"Delete: {product.Name}");
                            Products.Clear();
                            LoadProducts();
                        }
                        else // Update
                        {
                            //MessageBox.Show(product.ToString());
                            string command =
                                "UPDATE Products " +
                                $"SET Name = @name, Price = @price " +
                                $"WHERE Id=@id;";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", product.Id);
                            cmd.Parameters.AddWithValue("@name", product.Name);
                            cmd.Parameters.AddWithValue("@price", product.Price);
                            ExecuteCommand(cmd, "Update Department");
                            Products.Clear();
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
                if (item.Content is Entity.Manager manager)
                {
                    ManagerCrudWindow dialog = new()
                    {
                        Owner = this,
                        Manager = manager
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Manager is null) //Delete
                        {
                            string command =
                                @"UPDATE Managers
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE Id = @id; ";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", manager.Id);
                            ExecuteCommand(cmd, $"Delete: {manager.Name} {manager.Surname}");
                            Managers.Clear();
                            LoadManagers();
                        }
                        else // Update
                        {
                            string command =
                                @"UPDATE Managers 
                                SET 
                                Surname = @surname,
                                Name = @name, 
                                Secname = @secname, 
                                Id_main_dep = @IdMainDep, 
                                Id_sec_dep = @IdSecDep, 
                                Id_chief = @IdChief
                                WHERE Id = @id;";

                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", manager.Id);
                            cmd.Parameters.AddWithValue("@surname", manager.Surname);
                            cmd.Parameters.AddWithValue("@name", manager.Name);
                            cmd.Parameters.AddWithValue("@secname", manager.Secname);
                            cmd.Parameters.AddWithValue("@IdMainDep", manager.IdMainDep);
                            if (manager.IdSecDep != null)
                                cmd.Parameters.AddWithValue("@IdSecDep", manager.IdSecDep);
                            else
                                cmd.Parameters.AddWithValue("@IdSecDep", DBNull.Value);

                            if (manager.IdChief != null)
                                cmd.Parameters.AddWithValue("@IdChief", manager.IdChief);
                            else
                                cmd.Parameters.AddWithValue("@IdChief", DBNull.Value);

                            ExecuteCommand(cmd, "Update Manager");
                            Managers.Clear();
                            LoadManagers();
                        }
                    }

                }
            }
        }
        private void SalesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Sale sale)
                {
                    SaleCrudWindow dialog = new() { Owner = this, Sale = sale };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Sale is null)
                        {
                            string command =
                               @"UPDATE Sales
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE Id = @id; ";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", sale.Id);
                            var product = Products.Where(p => p.Id == sale.ProductId).FirstOrDefault();
                            ExecuteCommand(cmd, $"Delete sale info about: {product.Name}");
                            Sales.Clear();
                            LoadSales();
                        }
                        else
                        {
                            string command =
                               @"UPDATE Sales
                                 SET
                                 Quantity = @quantity, 
                                 Product_Id = @product_id,
                                 Manager_Id = @manager_id 
                                 WHERE Id = @id;";
                            using SqlCommand cmd = new(command, _connection);
                            cmd.Parameters.AddWithValue("@id", dialog.Sale.Id);
                            cmd.Parameters.AddWithValue("@quantity", dialog.Sale.Quantity);
                            cmd.Parameters.AddWithValue("@product_id", dialog.Sale.ProductId);
                            cmd.Parameters.AddWithValue("@manager_id", dialog.Sale.ManagerId);
                            ExecuteCommand(cmd, "Update Sale");
                            Sales.Clear();
                            LoadSales();
                        }
                    }
                }
            }
        }
        #endregion

        #region CREATE_NEW_ROWS_DB

        //СТВОРЕННЯ НОВОГО ВІДДІЛУ
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentCrudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Department is not null)
                {
                    String sql = "INSERT INTO Departments(Id, Name)" +
                     " VALUES(@id, @name)";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Department.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.Department.Name);
                    ExecuteCommand(cmd, "Add new Department");
                    Departments.Clear();
                    LoadDepartments();
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductCrudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Product is not null)
                {
                    /*
                        НЕ РЕКОМЕНДУЄТЬСЯ див. у конспект
                        String sql = $"INSERT INTO Products(Id, Name, Price)" +
                            $" VALUES('{dialog.Product.Id}', N'{dialog.Product.Name}', {dialog.Product.Price})";
                        ExecuteCommand(sql, "Add new Product");
                        Products.Clear();
                        LoadProducts();
                        MessageBox.Show(dialog.Product.ToString());
                    */

                    String sql = "INSERT INTO Products(Id, Name, Price)" +
                     " VALUES(@id, @name, @price)";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Product.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.Product.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.Product.Price);
                    ExecuteCommand(cmd, "Add new Product");
                    Products.Clear();
                    LoadProducts();
                }
            }
        }

        private void AddManagerButtoт_Click(object sender, RoutedEventArgs e)
        {
            ManagerCrudWindow dialog = new()
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Manager is not null)
                {
                    string command =
                               @"INSERT INTO Managers(Id, Surname, Name, Secname, Id_main_dep, Id_sec_dep, Id_chief) 
                                VALUES( 
                                @id, @surname, @name, @secname, @IdMainDep, @IdSecDep, @IdChief
                                );";

                    using SqlCommand cmd = new(command, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Manager.Id);
                    cmd.Parameters.AddWithValue("@surname", dialog.Manager.Surname);
                    cmd.Parameters.AddWithValue("@name", dialog.Manager.Name);
                    cmd.Parameters.AddWithValue("@secname", dialog.Manager.Secname);
                    cmd.Parameters.AddWithValue("@IdMainDep", dialog.Manager.IdMainDep);
                    if (dialog.Manager.IdSecDep != null)
                        cmd.Parameters.AddWithValue("@IdSecDep", dialog.Manager.IdSecDep);
                    else
                        cmd.Parameters.AddWithValue("@IdSecDep", DBNull.Value);

                    if (dialog.Manager.IdChief != null)
                        cmd.Parameters.AddWithValue("@IdChief", dialog.Manager.IdChief);
                    else
                        cmd.Parameters.AddWithValue("@IdChief", DBNull.Value);

                    ExecuteCommand(cmd, "Create Manager");
                    Managers.Clear();
                    LoadManagers();
                }
            }
        }

        private void AddSalesButton_Click(object sender, RoutedEventArgs e)
        {
            SaleCrudWindow dialog = new() { Owner = this, Sale = null };
            //dialog.ShowDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Sale is not null)
                {
                    string command =
                               @"INSERT INTO Sales(Id, SaleDate, Quantity, Product_Id, Manager_Id) 
                                VALUES( 
                                @id, @saledate, @quantity, @product_id, @manager_id
                                );";

                    using SqlCommand cmd = new(command, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Sale.Id);
                    cmd.Parameters.AddWithValue("@saledate", dialog.Sale.SaleDate);
                    cmd.Parameters.AddWithValue("@quantity", dialog.Sale.Quantity);
                    cmd.Parameters.AddWithValue("@product_id", dialog.Sale.ProductId);
                    cmd.Parameters.AddWithValue("@manager_id", dialog.Sale.ManagerId);
                    ExecuteCommand(cmd, "Create Manager");
                    Sales.Clear();
                    LoadSales();
                }
            }

        }

        #endregion

    }
}
