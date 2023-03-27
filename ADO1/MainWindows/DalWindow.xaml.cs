using ADO1.CRUDWindows;
using ADO1.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DalWindow : Window
    {
        private readonly DataContext dataContext;

        public ObservableCollection<Entity.Department> DepartmentsList { get; set; }
        public ObservableCollection<Entity.Manager> ManagersList { get; set; }

        public DalWindow()
        {
            InitializeComponent();
            dataContext = new();
            DepartmentsList = new(dataContext.Departments.GetAll());
            ManagersList = new(dataContext.Managers.GetAll());
            this.DataContext = this;
        }

        #region WINDOWS_EVENTS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(dataContext.Departments.GetAll().Count.ToString());
        }
        #endregion

        #region DOUBLE_CLICKS
        private void DepartmentsItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    DepartmentCrudWindow dialog = new DepartmentCrudWindow() { Department = department };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Department is null)
                        {
                            if (dataContext.Departments.Delete(department))
                            {
                                MessageBox.Show("Видалено успішно");
                                DepartmentsList.Remove(department);
                            }
                            else
                                MessageBox.Show("Помилка видалення");
                        }
                        else
                        {
                            if (dataContext.Departments.Update(dialog.Department))
                            {
                                MessageBox.Show("Оновлено успішно");
                                DepartmentsList.Add(dialog.Department);
                                DepartmentsList.Remove(department);
                            }
                            else
                                MessageBox.Show("Помилка оновлення");
                        }
                    }
                }
            }
        }

        private void ManagersItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    MessageBox.Show(manager.ToString());
                }
            }
        }
        #endregion

        #region ADD_NEW_ROWS
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentCrudWindow dialog = new();

            if (dialog.ShowDialog() == true)
            {
                if (dataContext.Departments.Add(dialog.Department))
                {
                    MessageBox.Show("Додано успішно");
                    DepartmentsList.Add(dialog.Department);
                }
                else
                    MessageBox.Show("Помилка додавання");
            }
        }

        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerCrudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {

            }
        }
        #endregion
    }
}
