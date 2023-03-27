using ADO1.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для ProductCrudWindow.xaml
    /// </summary>
    /// 
    public partial class ProductCrudWindow : Window
    {
        public Entity.Product Product { get; set; }

        private bool SaveButtonState;
        private bool inputWasChaged;
        private bool stringIsEmpty;
        private DispatcherTimer timer;

        public ProductCrudWindow()
        {
            InitializeComponent();
            Product = null;
            BaseOptions();
        }
        private void BaseOptions()
        {
            SaveButtonState = true;
            inputWasChaged = false;
            stringIsEmpty = false;

            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(CheckNameField);
            timer.Start();
        }

        #region CONDITIONS
        private void CheckNameField(object sender, EventArgs args)
        {
            if (NameView.Text == Product.Name)
            {
                SaveButtonState = false;
                ErrorText.Text = "*The text field contains the original value";
                inputWasChaged = false;
            }
            else
            {
                inputWasChaged = true;
                if (!stringIsEmpty)
                {
                    SaveButtonState = true;
                }
            }

            if (NameView.Text.Trim() == String.Empty || PriceView.Text.Trim() == String.Empty)
            {
                SaveButtonState = false;
                ErrorText.Text = "*Field is empty or contains only spaces";
                stringIsEmpty = true;
            }
            else
            {
                stringIsEmpty = false;
                if (inputWasChaged)
                {
                    SaveButtonState = true;
                }
            }
        }
        #endregion

        #region WINDOW_EVENTS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Product is null)  // режим додавання (Create)
            {
                Product = new() { Id = Guid.NewGuid() };

                WindowName.Text = "CREATE PRODUCT";
                CrudButtons.ColumnDefinitions.RemoveAt(1);
                CrudButtons.Children.Remove(DeleteButton);

            }
            else // режим редагування чи видалення (Update or Delete)
            {

                NameView.Text = Product.Name;
                PriceView.Text = Product.Price.ToString();
                DeleteButton.IsEnabled = true;
            }
            IdView.Text = Product.Id.ToString();
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
        //2ПАРА
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveButtonState)
            {
                Product.Name = NameView.Text;
                try
                {
                    Product.Price = double.Parse(
                        PriceView.Text.Replace(',', '.'),
                        CultureInfo.InvariantCulture);
                }
                catch
                {
                    MessageBox.Show("Неправильний формат числа для ціни");
                    return;
                }
                this.DialogResult = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                 $"Do you really want to remove: {Product.Name}",
                 "Delete field",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Product = null;
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
