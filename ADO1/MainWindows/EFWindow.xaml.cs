using ADO1.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ADO1.MainWindows
{
    /// <summary>
    /// Логика взаимодействия для EFWindow.xaml
    /// </summary>
    public partial class EFWindow : Window
    {
        internal EfContext efContext { get; set; } = new();
        private ICollectionView depListView;

        private static readonly Random random = new Random();

        public EFWindow()
        {
            InitializeComponent();
            this.DataContext = efContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            efContext.Departments.Load();
            depList.ItemsSource = efContext
                .Departments
                .Local
                .ToObservableCollection();
            //отримання посилання на depList, але як інтерфейс ICollectionView
            depListView = CollectionViewSource.GetDefaultView(depList.ItemsSource);
            depListView.Filter = //Predicate<object>
                obj => (obj as Department)?.DeleteDt == null;
            UpdateMonitor();
            UpdateDailyStatistics();
        }

        public void UpdateMonitor()
        {
            MonitorBlock.Text = "Departments: " +
                efContext.Departments.Count().ToString();
            MonitorBlock.Text += "\nProducts: " +
                efContext.Products.Count().ToString();
            MonitorBlock.Text += "\nManagers: " +
                efContext.Managers.Count().ToString();
            MonitorBlock.Text += "\nSales: " +
                efContext.Sales.Count().ToString();


        }

        private void UpdateDailyStatistics()
        {
            // Статистика продажів за сьогодні:
            // загалом продажів (чеків, записів у Sales) за сьогодні (усіх, у т.ч. видалених)
            SalesChecks.Content = efContext.Sales.Count();
            // загальна кількість проданих товарів (сума)
            SalesCnt.Content = efContext.Sales.Sum(s => s.Quantity);
            // фактичний час старту продажів сьогодні
            DateTime time = efContext.Sales.Min(s => s.SaleDate);
            StartMoment.Content = $"{time.Hour}:{time.Minute}:{time.Second}";
            // час останнього продажу
            time = efContext.Sales.Max(s => s.SaleDate);
            FinishMoment.Content = $"{time.Hour}:{time.Minute}:{time.Second}";
            // максимальна кількість товарів у одному чеку (за сьогодні)
            MaxCheckCnt.Content = efContext.Sales.Max(s => s.Quantity); ;
            // "середній чек" за кількістю - середнє значення кількості 
            //  проданих товарів на один чек
            AvgCheckCnt.Content = efContext.Sales.Average(s=>s.Quantity);
            // Повернення - чеки, що є видаленими (кількість чеків за сьогодні)
            DeletedCheckCnt.Content = efContext.Sales.Where(s => s.DeleteDt != null).Count();
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentCrudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {
                //dialog.Department -- інша сутність, треба замінити під EF
                efContext.Departments.Add(
                    new Department()
                    {
                        Name = dialog.Department.Name,
                        Id = dialog.Department.Id
                    }
                    );
                // !! Додавання даних до контексту не додає їх до БД -- планування додавання
                efContext.SaveChanges(); // внесення змін до БД

                MonitorBlock.Text += "\nDepartments: " +
                    efContext.Departments.Count().ToString();
            }
        }


        #region DOUBLE_CLICKS
        private void DepartmentItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Department department)
                {
                    DepartmentCrudWindow dialog = new();
                    dialog.Department = new Entity.Department()
                    {
                        Id = department.Id,
                        Name = department.Name
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Department is null)
                        {
                            department.DeleteDt = DateTime.Now;
                            depListView.Filter = DepartmentsDeletedFilter;
                            efContext.SaveChanges();
                        }
                        else
                        {
                            department.Name = dialog.Department.Name;
                            depList.Items.Refresh();
                            efContext.SaveChanges();
                        }
                    }

                }
            }
        }
        #endregion

        private bool DepartmentsDeletedFilter(object item)
        {
            if (item is Department department)
                return department.DeleteDt == null;
            return false;
        }

        private void ShowAllDepsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            depListView.Filter = null;// скидаємо фільтр -- показує усі дані

            ((GridView)depList.View) // Властивості Visivle для колонок ListView немає, тому
                .Columns[2]          // приховування/відображення через встановлення Width
                .Width = Double.NaN; // Double.NaN - автоматичне визначення
        }

        private void ShowAllDepsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //depListView.Filter = //Predicate<object>
            //    obj => (obj as Department)?.DeleteDt == null;
            depListView.Filter = DepartmentsDeletedFilter;
            ((GridView)depList.View).Columns[2].Width = 0;
        }

        private void GenerateSalesButton_Click(object sender, RoutedEventArgs e)
        {
            // Manager manager = - отримання .ToList() передає всі дані, для BigData неприйнятно
            // Manager manager = efContext.Managers.ToList()[random.Next(efContext.Managers.Count())];  //Випадковий менеджер з наявних

            // Manager manager = // System.InvalidOperationException - Linq to Entity перекладає запит на SQL
            // efContext.Managers.ElementAt(random.Next(efContext.Managers.Count())); // Не всі можливості С# мають аналоги у SQL

            // DbSet приймає метод розширення LINQ, але не всі вони врешті спрацюють, оскільки
            // це Linq-to-Entity (LINQ-to-SQL), що накладає певні обмеження
            double maxPrice = efContext.Products.Max(p => p.Price);
            int manCnt = random.Next(efContext.Managers.Count());
            int proCnt = random.Next(efContext.Products.Count());
            for (int i = 0; i < 100; i++)
            {
                //Випадковий менеджер з наявних
                Manager manager = efContext.Managers.Skip(manCnt).First(); 
                //Віипадковий продукт                                                        
                Product product = efContext.Products.Skip(proCnt).First();
                //Випадкова кількість, але чим дорожче товар, тим менша кількість
                int quantity = random.Next(1,
                    (int)(20 * (1 - product.Price / maxPrice) + 2));
                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Quantity = quantity,
                    //Дата "сьогодні" але з випадковим часом
                    SaleDate = DateTime.Today.AddSeconds(random.Next(86400))
                });
            }
            efContext.SaveChanges();
            UpdateMonitor();
        }
    }
}
