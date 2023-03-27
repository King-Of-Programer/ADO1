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
using System.Windows.Threading;

namespace ADO1.CRUDWindows
{
    /// <summary>
    /// Логика взаимодействия для SaleCrudWindow.xaml
    /// </summary>
    public partial class SaleCrudWindow : Window
    {

        public Entity.Sale? Sale;

        private ObservableCollection<Entity.Manager> OwnerManagers;
        private ObservableCollection<Entity.Product> OwnerProducts;

        private bool SaveButtonState;
        private DispatcherTimer timer;

        public SaleCrudWindow()
        {
            InitializeComponent();
            Sale = null;
            OwnerManagers = null;
            OwnerProducts = null;
            BaseOptions();
        }
        private void BaseOptions()
        {
            ErrorText.Visibility = Visibility.Hidden;
            SaveButtonState = true;

            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(CheckFields);
            timer.Start();
        }
        #region CONDITIONS
        private void CheckFields(object sender, EventArgs args)
        {

            if (QuantityView.Text.Trim() == String.Empty ||
                ProductsComboBox.SelectedItem is null ||
                ManagerComboBox.SelectedItem is null)
            {
                SaveButtonState = false;
                ErrorText.Text = "*Field is empty or contains only spaces";

            }
            else
            {
                SaveButtonState = true;
            }
        }
        #endregion

        #region WINDOW_EVENTS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is ORMWindow owner)
            {
                DataContext = Owner;
                OwnerManagers = owner.Managers;
                OwnerProducts = owner.Products;
            }
            else
            {
                MessageBox.Show("Owner is not OrmWindow");
                Close();
            }

            if (this.Sale is null)
            {
                //ID, Quantity, SaleDt генеруться у конструкторі
                Sale = new Entity.Sale();
                CrudButtons.ColumnDefinitions.RemoveAt(1);
                CrudButtons.Children.Remove(DeleteButton);
                WindowName.Text = "CREATE Sale";
            }
            else
            {
                ProductsComboBox.SelectedItem =
                    OwnerProducts
                    .Where(d => d.Id == this.Sale.ProductId)
                    .First();
                ManagerComboBox.SelectedItem =
                    OwnerManagers
                    .Where(d => d.Id == this.Sale.ManagerId)
                    .FirstOrDefault();
            }
            IdView.Text = Sale.Id.ToString();
            QuantityView.Text = this.Sale.Quantity.ToString();
            SaleDateView.Text = this.Sale.SaleDate.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        #endregion

        #region BUTTONS_EVENTS
        private void SaveButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!SaveButtonState)
            {
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!SaveButtonState)
            {
                ErrorText.Visibility = Visibility.Hidden;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveButtonState)
            {
                bool quantityCheck = true;
                int cnt = 0;
                try
                {
                    cnt = Convert.ToInt32(QuantityView.Text);
                }
                catch
                {
                    MessageBox.Show("Кількість не розпізнана (очікується число)");
                    quantityCheck = false;
                }

                if (quantityCheck)
                {
                    this.Sale.Quantity = cnt;
                    if (ManagerComboBox.SelectedItem is Entity.Manager manager)
                        this.Sale.ManagerId = manager.Id;
                    else MessageBox.Show("ManagerComboBox.SelectedItem CAST Error");

                    if (ProductsComboBox.SelectedItem is Entity.Product product)
                        this.Sale.ProductId = product.Id;
                    else MessageBox.Show("ProductsComboBox.SelectedItem CAST Error");

                    this.DialogResult = true;
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                 $"Do you really want to remove sale info about: {(ProductsComboBox.SelectedItem as Entity.Product).Name}",
                 "Delete field",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Sale = null;
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // те що поверне ShowDialog
        }

        #endregion

    }
}
