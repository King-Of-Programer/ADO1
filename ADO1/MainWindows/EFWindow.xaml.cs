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
    }
}
