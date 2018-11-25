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
    /// Interaction logic for OknoDodawaniaUzytkownika.xaml
    /// </summary>
    public partial class OknoDodawaniaUzytkownika : Window
    {
        public Uzytkownik NowyUzytkownik { get; set; }
        public string Haslo { get; set; }
        public OknoDodawaniaUzytkownika()
        {
            InitializeComponent();
            uprawnieniaBox.ItemsSource = Enum.GetNames(typeof (Uprawnienia)).ToList().Where(s => s != "Anonymous");
            uprawnieniaBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            NowyUzytkownik = new Uzytkownik();
            NowyUzytkownik.Imie = imieBox.Text;
            NowyUzytkownik.Nazwisko = nazwiskoBox.Text;
            NowyUzytkownik.Login = loginBox.Text;
            Haslo = hasloBox.Password;
            NowyUzytkownik.Uprawnienia = (uprawnieniaBox.Text == "User"
                ? Uprawnienia.Uzytkownik
                : Uprawnienia.Administrator);
            if (imieBox.Text.Trim() == "" || nazwiskoBox.Text.Trim() == "" || loginBox.Text.Trim() == "")
            {
                MessageBox.Show("Firstname, lastname and login field must be filled.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            else Close();
        }
        
    }
}
