using ADO1.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADO1.MainWindows
{
    /// <summary>
    /// Логика взаимодействия для EFWindow.xaml
    /// </summary>
    public partial class EFWindow : Window
    {
        public EfContext efContext { get; set; } = new();
        private ICollectionView depListView;

        private static readonly Random random = new Random();

        public EFWindow()
        {
            InitializeComponent();
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
            #region OneTablesQueries
            // Статистика продажів за сьогодні:
            // загалом продажів (чеків, записів у Sales) за сьогодні (усіх, у т.ч. видалених)
            SalesChecks.Content = efContext.Sales.Count();
            
            // загальна кількість проданих товарів (сума)
            SalesCnt.Content = efContext.Sales.Sum(s => s.Quantity);
            
            // фактичний час старту продажів сьогодні
            DateTime time = efContext.Sales.Min(s => s.SaleDt);
            StartMoment.Content = $"{time.Hour}:{time.Minute}:{time.Second}";
            
            // час останнього продажу
            time = efContext.Sales.Max(s => s.SaleDt);
            FinishMoment.Content = $"{time.Hour}:{time.Minute}:{time.Second}";
            
            // максимальна кількість товарів у одному чеку (за сьогодні)
            MaxCheckCnt.Content = efContext.Sales.Max(s => s.Quantity); ;
            
            // "середній чек" за кількістю - середнє значення кількості 
            //  проданих товарів на один чек
            AvgCheckCnt.Content = efContext.Sales.Average(s => s.Quantity);
            
            // Повернення - чеки, що є видаленими (кількість чеків за сьогодні)
            DeletedCheckCnt.Content = efContext.Sales.Where(s => s.DeleteDt != null && s.SaleDt == DateTime.Today).Count();
            #endregion

            #region BadQuery
            //var query = efContext.Sales
            //    .Where(s=>s.SaleDt.Date==DateTime.Today)
            //    .GroupBy(s => s.ProductId);// групування за s.ProductId

            //foreach (IGrouping<Guid,Sale> grp in query)
            //{
            //    LogBlock.Text += grp.Key.ToString() + " " +// Guid - s.ProductId
            //        grp.Count() + "\n";
            //        // grp - це коллекція Sale, що має однаковий grp.Key (ProductId)
            //}

            //var query2 = efContext.Sales
            //   .Where(s => s.SaleDt.Date == DateTime.Today)
            //   .GroupBy(s => s.ProductId)   //  Після .GroupBy утворюється "коллекція" IGrouping<Guid,Sale> grp
            //   .ToList()
            //   .Join(                       //  .Join відбувається не з Sales, а з grp
            //        efContext.Products,     //  1) з чим поэднуэмо (inner)
            //        grp => grp.Key,         //  2) outerKey - ключ з "grp"
            //        p => p.Id,              //  3) innerKey - ключ з Products (p)
            //        (grp, p) => new         //  4) resultSelector - правило за яким
            //        {                       //      з поєднаної пари (grp, p) утворюється нова
            //            Name = p.Name,      //      послідовність ("колекція") - новий об'єкт
            //            Cnt = grp.Count()   //      анонімного типу
            //        }                       //
            //    );// групування за s.ProductId
            //LogBlock.Text = "";
            //foreach (var item in query2)
            //{
            //    LogBlock.Text += $"{item.Name} -- {item.Cnt}\n";
            //}
            #endregion

            #region TwoTablesQueries
            /* Д.З. Написати запити для визначення кращого товару
             * а) за кількістю чеків (класна робота)
             * б) за кількістю проданих шт
             * в) за сумою продажів 
             * Разом з назвою вивести також числову хар-ку (шт/грн)
             */
            //ChecksCount
            var query1 = efContext.Products
               .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                     p => p.Id,
                     s => s.ProductId,
                     (p, sales) => new
                     {
                         Name = p.Name,
                         Cnt = sales.Count()
                     }
                ).OrderByDescending(g => g.Cnt);
            //foreach (var item in query1)
            //{
            //    LogBlock.Text += $"{item.Name} -- {item.Cnt}\n";
            //}
            BestProduct.Content = query1
                .First().Name+" "+query1.First().Cnt+"pcs.";

            //SaleQuantity
            var query2 = efContext.Products
               .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                     p => p.Id,
                     s => s.ProductId,
                     (p, sales) => new
                     {
                         Name = p.Name,
                         QuantitySum = sales.Sum(s=>s.Quantity)
                     }
                ).OrderByDescending(g => g.QuantitySum);
            BestProductCnt.Content = query2
                .First().Name + " " + query2.First().QuantitySum+"pcs.";

            //Sum
            var query3 = efContext.Products
                .GroupJoin(
                    efContext.Sales.Where(s=>s.SaleDt.Date==DateTime.Today),
                    p => p.Id,
                    s => s.ProductId,
                    (p, sales) => new
                    {
                        Name = p.Name,
                        Sum = sales.Sum(s => s.Quantity) * p.Price
                    }
                ).OrderByDescending(g=>g.Sum);
            BestProductSum.Content = query3.First().Name + " " + query3.First().Sum +"grn.";

            #endregion

            #region BestManagerChecks
            var bestManagerCh = efContext.Managers
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,           //  У разі коли у вибірці потрібна
                    (m, sales) => new           //  комплексна характеристика - нормальна
                    {                           //  пракитка включати посилання на саму
                        Manager = m,            //  сутність (Managers = m). У той же час
                        Cnt = sales.Count()     //  множини (sales) слід обробляти, оскільки
                    }                           //  це не колекції, а правила запитів, які
                )                               //  втрачають контекст після закінчення GroupJoin
                .OrderByDescending(g => g.Cnt)
                .First(); //  Якщо потрібна множина продаж, то sales.ToList()
            BestManagerChecks.Content = 
                bestManagerCh.Manager.Surname +" "+
                bestManagerCh.Manager.Name[0] +". "+
                bestManagerCh.Manager.Secname[0] +". -- "+
                bestManagerCh.Cnt;
            #endregion

            #region BestManagerTop
            var bestManagerTop = efContext.Managers
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, sales) => new
                    {
                        Manager = m,
                        Cnt = sales.Count(),
                        Quantity = sales.Sum(s => s.Quantity)
                    }
                )
                .OrderByDescending(g => g.Quantity)
                .Take(3)
                .ToList();
            BestManagersTop.Content = "";
            int pos = 0;
            foreach(var manager in bestManagerTop)
            {
                BestManagersTop.Content += $"{pos + 1}. " +
                manager.Manager.Surname + " " +
                manager.Manager.Name[0] + ". " +
                manager.Manager.Secname[0] + ". -- " +
                manager.Quantity + "\n";
                pos++;
            }
            #endregion

            #region BestManagerMoneySchemeA
            /*
             Cхема A:
             Managers		Sales		   Products
             Id ----------- ManagerId	   Name
              |			 /	ProductId------Id
              |			/	Quantity	   Price
              |		   /		\           /
               GroupJoin	     \	       /
             	Man			Money = Quantity * Price
             	Sales
             */
            //BestManagerMoney.Content = "";
            //var queryMoney = efContext.Managers
            //   .GroupJoin(
            //        efContext.Sales
            //        .Where(s => s.SaleDt.Date == DateTime.Today)
            //        .Join(
            //            efContext.Products,
            //            sale => sale.ProductId,
            //            product => product.Id,
            //            (sale, product) => new
            //            {
            //                ManagerId = sale.ManagerId,
            //                CheckSum = sale.Quantity * product.Price
            //            }),
            //        m => m.Id,
            //        s => s.ManagerId,
            //        (m, pricedChecks) => new
            //        {
            //            Manager = m,
            //            Cnt = pricedChecks.Sum(c => c.CheckSum)
            //        }

            //    )
            //   .OrderByDescending(g => g.Cnt)
            //   .First();
            //BestManagerMoney.Content =
            //    queryMoney.Manager.Surname + " " +
            //    queryMoney.Manager.Name[0] + ". " +
            //    queryMoney.Manager.Secname[0] + ". -- " +
            //    queryMoney.Cnt.ToString("0.00") + " UAH";

            #endregion

            #region BestManagerMoneySchemeB
            /*
             Cхема В:
             Managers		Sales
              Id	------------- ManagerId
              |				/ ProductId
              |			   /  Quantity
             	GroupJoin
             	Man						Products
             	Sales					 Name
	             	ProductId --------	 Id
	             	Quantity			 Price
	             			\			/
	             			 \		   /
	             			Money = Quantity * Price
             */
            BestManagerMoney.Content = "";
            var queryMoney = efContext.Managers
               .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                     m => m.Id,
                     s => s.ManagerId,
                     (m, sales) => new
                     {
                         Manager = m,
                         Cnt = sales
                            .Join(
                                efContext.Products,
                                sale => sale.ProductId,
                                product => product.Id,
                                (sale, product) => sale.Quantity * product.Price)
                            .Sum()
                     }
                ).OrderByDescending(g => g.Cnt).First();
            BestManagerMoney.Content =
                queryMoney.Manager.Surname + " " +
                queryMoney.Manager.Name[0] + ". " +
                queryMoney.Manager.Secname[0] + ". -- " +
                queryMoney.Cnt.ToString("0.00") + " UAH";
            #endregion

            #region Departments
            /*
             Select d.Name as DepartmentName, 
             SUM(p.Price*s.Quantity) as SumOfAllSales, 
             COUNT(s.ProductId) as ChecksCount, 
             SUM(s.Quantity) as TotalQuantity
             
             from Departments as d
             
             INNER JOIN Managers as m
             on d.Id=m.IdMainDep
             Inner Join Sales as s
             on s.ManagerId=m.Id
             Inner Join Products as p
             on s.ProductId=p.Id
             
             GROUP BY d.Name
             Order by SumOfAllSales DESC
             
             */
            var departmentsStats = efContext.Departments.ToList()
                 .GroupJoin(
                 efContext.Managers
                 .GroupJoin(
                 efContext.Sales,
                 manager => manager.Id,
                 sale => sale.ManagerId,
                 (manager, sales) => new
                 {
                     Manager = manager,
                     Cnt = sales.Count(),
                     Sum = sales.Sum(s => s.Quantity),
                     Money = sales.Join(
                                efContext.Products,
                                sale => sale.ProductId,
                                product => product.Id,
                                (sale, product) => sale.Quantity * product.Price)
                            .Sum()
                 }),
                 d => d.Id,
                 m => m.Manager.IdMainDep,
                 (dep, managers) => new
                 {
                     Department = dep,
                     Cnt = managers.Sum(m => m.Cnt),
                     Sum = managers.Sum(m => m.Sum),
                     Money = managers.Sum(m => m.Money)
                 }).OrderByDescending(d => d.Money);

            Deps.Content = "";
            foreach (var department in departmentsStats)
            {
                Deps.Content += $"{department.Department.Name} -- {department.Cnt} -- {department.Sum} -- {department.Money.ToString("0.00")}\n";
            }
            #endregion

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
            // Випадковий менеджер з наявних
            // Manager manager =  // "-" отримання .ToList() передає всі дані, для BigData неприйнятно
            //    efContext.Managers.ToList()[random.Next(efContext.Managers.Count())];
            // Manager manager = // System.InvalidOperationException - LINQ-to-Entity перекладає запит на SQL. Не всі 
            //    efContext.Managers.ElementAt(random.Next(efContext.Managers.Count())); // можливості мови C# мають аналоги у SQL
            // DbSet приймає методи розширення LINQ, але не всі вони врешті спрацьовують, оскільки
            //    це LINQ-to-Entity (LINQ-to-SQL), що накладає певні обмеження

            double maxPrice = efContext.Products.Max(p => p.Price);
            int manCnt = efContext.Managers.Count();
            int proCnt = efContext.Products.Count();

            for (int i = 0; i < 100; i++)
            {
                Manager manager = efContext.Managers.Skip(random.Next(manCnt)).First();
                // Випадковий товар
                Product product = efContext.Products.Skip(random.Next(proCnt)).First();
                // Випадкова кількість, але чим дорожче товар, тим менша гранична кількість
                int quantity = random.Next(1,
                    (int)(20 * (1 - product.Price / maxPrice) + 2));

                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Quantity = quantity,
                    SaleDt = DateTime.Today.AddSeconds(random.Next(0, 86400))  // Дата "сьогодні" але з випадковим часом
                });
            }
            efContext.SaveChanges();
            UpdateMonitor();
            UpdateDailyStatistics();
        }
    }
}
