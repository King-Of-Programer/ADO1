using System;
using System.Collections.Generic;
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
    /// Interaction logic for DepartmentCrudWindow.xaml
    /// </summary>
    public partial class DepartmentCrudWindow : Window
    {
        public Entity.Department Department { get; set; }

        private bool SaveButtonState;
        private bool inputWasChaged;
        private bool stringIsEmpty;
        private DispatcherTimer timer;
        public DepartmentCrudWindow()
        {
            InitializeComponent();
            Department = null;
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

        #region WINDOW_EVENTS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Department is null)  // режим додавання (Create)
            {
                DeleteButton.IsEnabled = false;
            }
            else // режим редагування чи видалення (Update or Delete)
            {
                IdView.Text = Department.Id.ToString();
                NameView.Text = Department.Name;
                DeleteButton.IsEnabled = true;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }
        #endregion

        #region CONDITIONS
        private void CheckNameField(object sender, EventArgs args)
        {
            if (NameView.Text == Department.Name)
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

            if (NameView.Text.Trim() == String.Empty)
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
                Department.Name = NameView.Text;
                this.DialogResult = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Do you really want to remove: {Department.Name}",
                "Delete field",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Department = null;
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
