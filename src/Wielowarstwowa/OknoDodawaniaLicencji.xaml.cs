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
using Wielowarstwowa.SerwerLicencji;

namespace Wielowarstwowa
{
    /// <summary>
    /// Interaction logic for OknoDodawaniaLicencji.xaml
    /// </summary>
    public partial class OknoDodawaniaLicencji : Window
    {
        public int? IdUzytkownika { get; set; }
        public int? IdProgramu { get; set; }
        public OknoDodawaniaLicencji()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (uzytkownikBox.SelectedIndex == -1 || programBox.SelectedIndex == -1 || data.SelectedDate == null)
            {
                MessageBox.Show("Not all fields are filled.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            else
            {
                IdUzytkownika = uzytkownikBox.SelectedIndex;
                IdProgramu = programBox.SelectedIndex;
                Close();
            }
        }
    }
}
