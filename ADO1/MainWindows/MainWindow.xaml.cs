using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            //!! Створення об'єкта не відкриває підключення
            _connection = new SqlConnection();
            // Головний параметр підключення - рядок підключення
            _connection.ConnectionString = App.ConnectionString;
        }

        #region WINDOW_EVENTS

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                StatusConnection.Content = "Connected";
                StatusConnection.Foreground = Brushes.Green;
            }
            catch (SqlException ex)
            {
                StatusConnection.Content = "Disconnected";
                StatusConnection.Foreground = Brushes.Green;
                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowMonitor();
            ShowDepartments();
            ShowManagers();
            ShowProducts();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (_connection?.State == System.Data.ConnectionState.Open)
                _connection.Close();
        }

        #endregion

        #region Запити без повернення результатів

        #region INSTALL_DB
        private void installDepartments_Click(object sender, RoutedEventArgs e)
        {
            //Команда - інструмент для виконання SQL запитів
            SqlCommand cmd = new SqlCommand();
            //Головні параметри команди:
            cmd.Connection = _connection; //підключення (відкрите)
            cmd.CommandText = // SQL запит (текст)
                @"CREATE TABLE Departments(
	                Id			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                Name		NVARCHAR(50) NOT NULL
                    DeleteDt    DATETIME
                )";

            try//виконання команди
            {
                cmd.ExecuteNonQuery(); // NonQuery - без повернення результату
                MessageBox.Show("Create OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose(); // некерований ресурс, вимагає утилізації
        }
        private void installProducts_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = @"
                CREATE TABLE Products (
                Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                Name NVARCHAR(50) NOT NULL,
                Price FLOAT NOT NULL
                DeleteDt DATETIME);";

            try
            {
                cmd.ExecuteNonQuery();
                ShowMonitorProducts();
                MessageBox.Show(
                    "Products create",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void installManagers_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = @"CREATE TABLE Managers (
	            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	            Surname		NVARCHAR(50) NOT NULL,
	            Name		NVARCHAR(50) NOT NULL,
	            Secname		NVARCHAR(50) NOT NULL,
	            Id_main_dep UNIQUEIDENTIFIER NOT NULL REFERENCES Departments( Id ),
	            Id_sec_dep	UNIQUEIDENTIFIER REFERENCES Departments( Id ),
	            Id_chief	UNIQUEIDENTIFIER
                DeleteDt    DATETIME);";

            try
            {
                cmd.ExecuteNonQuery();
                ShowMonitorManagers();
                MessageBox.Show(
                    "Managers create",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        #endregion

        #region INSERT_DB
        private void insertlDepartments_Click(object sender, RoutedEventArgs e)
        {
            //Команда - інструмент для виконання SQL запитів
            SqlCommand cmd = new SqlCommand();
            //Головні параметри команди:
            cmd.Connection = _connection; //підключення (відкрите)
            cmd.CommandText = // SQL запит (текст)
                @"INSERT INTO Departments 
	                        ( Id, Name )
                VALUES 
	            ( 'D3C376E4-BCE3-4D85-ABA4-E3CF49612C94',  N'IT отдел'		 	 ), 
	            ( '131EF84B-F06E-494B-848F-BB4BC0604266',  N'Бухгалтерия'		 ), 
	            ( '8DCC3969-1D93-47A9-8B79-A30C738DB9B4',  N'Служба безопасности'), 
	            ( 'D2469412-0E4B-46F7-80EC-8C522364D099',  N'Отдел кадров'		 ),
	            ( '1EF7268C-43A8-488C-B761-90982B31DF4E',  N'Канцелярия'		 ), 
	            ( '415B36D9-2D82-4A92-A313-48312F8E18C6',  N'Отдел продаж'		 ), 
	            ( '624B3BB5-0F2C-42B6-A416-099AAB799546',  N'Юридическая служба' )";

            try//виконання команди
            {
                cmd.ExecuteNonQuery(); // NonQuery - без повернення результату
                MessageBox.Show("Insert OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose(); // некерований ресурс, вимагає утилізації
        }
        private void insertlProducts_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = @"
            INSERT INTO Products
        	( Id, Name,	Price	)
            VALUES
            ( 'DA1E17BB-A90D-4C79-B801-5462FB070F57', N'Гвоздь 100мм',			10.50	),
            ( 'A8E6BE17-5447-4804-AB61-F31ABF5A76D3', N'Шуруп 4х35',			4.25	),
            ( '21B0F444-2E4F-47D8-80C1-E69BF1C34CA8', N'Гайка М4',				6.50	),
            ( '2DCA5E44-B06D-4613-BB6A-D3BC91430BFE', N'Гровер М4',			    5.99	),
            ( '64A4DF8A-0733-4BE9-AABA-C01B4EC3612A', N'Болт 4х60',			    9.98	),
            ( 'B6D20749-B495-4B1A-BA1C-80B88E78B7CD', N'Гвоздь 80мм',			19.98	),
            ( '7B08197B-C55F-4389-891F-BF12A575DFFB', N'Отвертка PZ2',			35.50	),
            ( '870DA1A9-44F4-4018-B7FC-727A2058FAF0', N'Шуруповерт',			799		),
            ( '8FF90E21-DCDB-4D55-A557-7C6D57DBB029', N'Молоток',				216.50	),
            ( 'F7F1E576-AF8D-4749-869E-4A794FE69D42', N'Набор Новосел',		52.40	),
            ('BB29F63D-1261-41F2-89E8-88F44D5EC409', N'Сверло 6х80', 39.98),
            ('D17A4442-0A71-4673-B450-36929048ADEF', N'Шуруп 5х45',			5.98    ),
            ('69B125D7-99CC-42D6-A6FA-46687F333749', N'Винт потай 3х16',		3.98    ),
            ('94BC671A-A6B6-417A-BC9F-8AE4871A58EC', N'Дюбель 6х60',			5.50    ),
            ('EFC6578A-00B7-4766-A7E3-79CDBA8C294B', N'Органайзер для шурупов',199     ),
            ('9654271B-AB52-4225-A30C-D75054B1733F', N'Лазерный дальномер',	1950    ),
            ('F2585221-1ACA-4EFE-A5E8-C2F4534D1F92', N'Дрель электрическая',	990     ),
            ('4A550D3B-D1F2-40EF-AE4E-963612C6713A', N'Сварочный аппарат',		2099    ),
            ('17DB11D1-F50E-4CF4-9C54-CF1BD45802EA', N'Электроды 3мм',			49.98   ),
            ('7264D33A-16B9-4E22-B3F1-63D6DAE60078', N'Паяльник 40 Вт',		199.98  )";
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show(
                    "Products filled",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void insertlManagers_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = @"";
            try
            {
                cmd.CommandText = File.ReadAllText("Managers.sql");
                cmd.ExecuteNonQuery();
                ShowMonitorManagers();
                MessageBox.Show(
                    "Managers filled",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "IO error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }
        #endregion

        #region DROP_DB
        private void dropDepartments_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new("drop table Departments", _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show(
                    "Departments dropped",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  ex.Message + " Delete Managers first",
                  "Drop error",
                  MessageBoxButton.OK,
                  MessageBoxImage.Exclamation);
            }
        }

        private void dropProducts_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new("drop table Products", _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show(
                    "Products dropped",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  ex.Message,
                  "Drop error",
                  MessageBoxButton.OK,
                  MessageBoxImage.Exclamation);
            }
        }

        private void dropManagers_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new("drop table Managers", _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show(
                    "Managers dropped",
                    "SQL complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  ex.Message,
                  "Drop error",
                  MessageBoxButton.OK,
                  MessageBoxImage.Exclamation);
            }
        }
        #endregion

        #endregion

        #region Скалярні запити - з одним результатом
        private void ShowMonitor()
        {
            ShowMonitorDepartments();
            ShowMonitorProducts();
            ShowMonitorManagers();
        }
        // Відображає на моніторі кількість відділів (департементів) у БД
        private void ShowMonitorDepartments()
        {
            using SqlCommand cmd = new SqlCommand("select Count(*) from Departments", _connection);
            try
            {
                var res = cmd.ExecuteScalar(); //повертає "лівий-верхній" результат скаляру
                //тип повернення - object, оскільки результат може бути довільного типу
                //для використання результат бажано конвертувати до очікуваного типу
                int cnt = Convert.ToInt32(res);
                StatusDepartments.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusDepartments.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Cast error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                StatusDepartments.Content = "--";
            }
        }
        // Відображає на моніторі кількість продуктів у БД
        private void ShowMonitorProducts()
        {
            using SqlCommand cmd = new SqlCommand("select Count(*) from Products", _connection);
            try
            {
                var res = cmd.ExecuteScalar(); //повертає "лівий-верхній" результат скаляру
                //тип повернення - object, оскільки результат може бути довільного типу
                //для використання результат бажано конвертувати до очікуваного типу
                int cnt = Convert.ToInt32(res);
                StatusProducts.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusProducts.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Cast error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                StatusProducts.Content = "--";
            }
        }
        // Відображає на моніторі кількість менеджерів у БД
        private void ShowMonitorManagers()
        {
            using SqlCommand cmd = new SqlCommand("select Count(*) from Managers", _connection);
            try
            {
                var res = cmd.ExecuteScalar(); //повертає "лівий-верхній" результат скаляру
                //тип повернення - object, оскільки результат може бути довільного типу
                //для використання результат бажано конвертувати до очікуваного типу
                int cnt = Convert.ToInt32(res);
                StatusManagers.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "SQL error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusManagers.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Cast error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                StatusManagers.Content = "--";
            }
        }

        #endregion

        #region Запити із результатами

        #region ЕЛЕМЕНТИ_КЕРУВАННЯ
        private String shortString(string text)
        {
            string result = "";
            result += text.Substring(0, 4);
            result += "...";
            int length = text.Length - 1;
            result += text.Substring(length - 3);
            return result;
        }
        TextBlock createTextBlock(string text)
        {
            TextBlock block = new TextBlock();
            block.Text = text;
            block.TextAlignment = TextAlignment.Center;
            block.TextWrapping = TextWrapping.Wrap;
            block.FontSize = 15;
            return block;
        }
        #endregion

        //МАЛЮЄ ТАБЛИЦЮ ДЛЯ МЕНЕДЖЕРІВ
        void ManagersViewer(SqlDataReader reader, int count)
        {
            RowDefinition row = new RowDefinition();
            Viewer.RowDefinitions.Add(row);
            for (int i = 0; i < 4; i++)
            {
                var textBlock = createTextBlock(reader.GetString(i));
                Grid.SetRow(textBlock, count);
                Grid.SetColumn(textBlock, i);
                Viewer.Children.Add(textBlock);
            }
        }
        //МАЛЮЄ ТАБЛИЦЮ ДЛЯ ПРОДУКТІВ
        void ProductViewer(SqlDataReader reader, int count)
        {
            RowDefinition row = new RowDefinition();
            ProductsViewer.RowDefinitions.Add(row);
            var textBlock = createTextBlock(shortString(reader.GetGuid(0).ToString()));
            Grid.SetRow(textBlock, count);
            Grid.SetColumn(textBlock, 0);
            ProductsViewer.Children.Add(textBlock);

            textBlock = createTextBlock(reader.GetString(1));
            Grid.SetRow(textBlock, count);
            Grid.SetColumn(textBlock, 1);
            ProductsViewer.Children.Add(textBlock);

            textBlock = createTextBlock(reader.GetDouble(2).ToString());
            Grid.SetRow(textBlock, count);
            Grid.SetColumn(textBlock, 2);
            ProductsViewer.Children.Add(textBlock);

        }
        //ВІДОБРАЖАЄ ТАБЛИЦЮ З МЕНЕДЖЕРАМИ
        private void ShowManagers()
        {
            using SqlCommand cmd = new("SELECT Surname, Managers.Name, Secname, Departments.Name FROM Managers INNER JOIN Departments ON Managers.Id_sec_dep=Departments.Id", _connection);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    ManagersViewer(reader, count);
                }
                reader.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Query error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        //ВІДОБРАЖАЄ ТАБЛИЦЮ З ПРОДУКТАМИ
        private void ShowProducts()
        {
            using SqlCommand cmd = new("SELECT * FROM Products", _connection);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    ProductViewer(reader, count);
                }
                reader.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Query error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        //ВІДОБРАЖАЄ ДЕПАРТМЕНТИ
        private void ShowDepartments()
        {
            using SqlCommand cmd = new("SELECT * FROM Departments", _connection);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                /* reader - інструмент для передачі даних від БД до програми.
                 * Принцип - передача по одному рядку (табличного результату)
                 * команда reader.Read() передає рядок. Дані залишаются у reader
                 * Для доступу до даних викорситовується:
                 * а) get-тери на кшталт reader.GetGuid(0);
                 * б) індексатори типу reader[1]
                 * індекси(0,1) - це порядкові індекси полів у запиті
                 */
                string str = "";
                while (reader.Read())
                {
                    str += shortString(reader.GetGuid(0).ToString()) + " " + reader.GetString(1) + "\n";
                }
                ViewDepartments.Text = str;
                reader.Close(); // не закритий reader блокує інші команди
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Query error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
