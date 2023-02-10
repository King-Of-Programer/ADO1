using ADO1.Entity;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for NewDepartmentWindow.xaml
    /// </summary>
    public partial class NewDepartmentWindow : Window
    {
        private bool SaveButtonState;
        private bool inputWasChaged;
        private bool stringIsEmpty;
        private DispatcherTimer timer;

        public Entity.Department Department { get; set; }

        public NewDepartmentWindow()
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
        private void CheckNameField(object sender, EventArgs args)
        {
            
           
                inputWasChaged = true;
                if (!stringIsEmpty)
                {
                    SaveButtonState = true;
                    SaveButton.Background = Brushes.DarkGreen;
                    SaveButton.Foreground = Brushes.White;
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

        private void generateIdButton_Click(object sender, RoutedEventArgs e)
            {
                IdView.Text = Guid.NewGuid().ToString();
            }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveButtonState)
            {
                Department.Id = Guid.Parse(IdView.Text);
                Department.Name = NameView.Text;
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Department = null;
            this.DialogResult = false;

        }
        




       
    }
}

