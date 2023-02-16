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

namespace ADO1
{
    /// <summary>
    /// Interaction logic for ProductCudWindow.xaml
    /// </summary>
    public partial class ProductCudWindow : Window
    {
        public Entity.Product Product { get; set; }

        private bool SaveButtonState;
        private bool inputWasChaged;
        private bool stringIsEmpty;
        private DispatcherTimer timer;

        public ProductCudWindow()
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
                SaveButton.Background = Brushes.Gray;
                SaveButton.Foreground = Brushes.Black;
                ErrorText.Text = "*The text field contains the original value";
                inputWasChaged = false;
            }
            else
            {
                inputWasChaged = true;
                if (!stringIsEmpty)
                {
                    SaveButtonState = true;
                    SaveButton.Background = Brushes.DarkGreen;
                    SaveButton.Foreground = Brushes.White;
                }
            }

            if (NameView.Text.Trim() == String.Empty || Price.Text.Trim() == String.Empty)
            {
                SaveButtonState = false;
                SaveButton.Background = Brushes.Gray;
                SaveButton.Foreground = Brushes.Black;
                ErrorText.Text = "*Field is empty or contains only spaces";
                stringIsEmpty = true;
            }
            else
            {
                stringIsEmpty = false;
                if (inputWasChaged)
                {
                    SaveButtonState = true;
                    SaveButton.Background = Brushes.DarkGreen;
                    SaveButton.Foreground = Brushes.White;
                }
            }
        }


        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Product is null)  // режим додавання (Create)
            {
                Product = new() { Id = Guid.NewGuid() };
                DeleteButton.Visibility = Visibility.Collapsed;
            }
            else // режим редагування чи видалення (Update or Delete)
            {

                NameView.Text = Product.Name;
                Price.Text = Product.Price.ToString();
                DeleteButton.IsEnabled = true;
            }
            IdView.Text = Product.Id.ToString();
        }





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
                        Price.Text.Replace(',', '.'),
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
            Product = null;
            this.DialogResult = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // те що поверне ShowDialog
        }

    }
}

