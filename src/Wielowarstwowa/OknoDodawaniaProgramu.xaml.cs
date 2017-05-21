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

namespace Wielowarstwowa
{
    /// <summary>
    /// Interaction logic for OknoDodawaniaProgramu.xaml
    /// </summary>
    public partial class OknoDodawaniaProgramu : Window
    {
        public string Nazwa { get; set; }
        public OknoDodawaniaProgramu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (nazwaBox.Text.Trim() == "")
            {
                MessageBox.Show("Nazwa nie może być pusta.", "Ostrzeżenie", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            else
            {
                Nazwa = nazwaBox.Text;
                Close();
            }
        }
    }
}
