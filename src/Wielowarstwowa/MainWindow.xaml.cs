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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wielowarstwowa.SerwerLicencji;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Wielowarstwowa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChannelFactory<ILicencjeChannel> factory = new ChannelFactory<ILicencjeChannel>(new NetTcpBinding());
        private ILicencjeChannel channel;

        public List<Uzytkownik> Uzytkownicy { get; set; } = new List<Uzytkownik>();
        public List<Program> Programy { get; set; } = new List<Program>();
        public List<Licencja> Licencje { get; set; } = new List<Licencja>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void conBtn_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Type login first!", "Logging error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (channel != null )
            {
                try
                {
                    channel.Close();
                }
                catch (Exception) { }
                Wylogowany();
            }
            else
            {
                factory.Endpoint.Address = new EndpointAddress("net.tcp://" + addressTextBox.Text);
                channel = factory.CreateChannel();
                if (channel.State == CommunicationState.Opened)
                {
                    MessageBox.Show("Client is already connected.", "Informantion", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    try
                    {
                        channel.Open();
                        
                        
                        switch (channel.Logowanie(loginTextBox.Text, passwordBox.Password))
                        {
                            case WiadomoscZwrotna.Niepomyslnie:
                                MessageBox.Show("Logging failed.", "Logging error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                channel.Close();
                                Wylogowany();
                                break;
                            case WiadomoscZwrotna.Pomyslnie:
                                Zalogowany();
                                channel.Closed += (o, args) => Wylogowany();
                                channel.Faulted += (o, args) => Wylogowany();
                                break;
                        }
                    }
                    catch (Exception f)
                    {
                        MessageBox.Show("Cannot establish connection, reason: " + f.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                
            }
        }

        private void Zalogowany()
        {
            conBtn.Content = "Disconnect";
            addressTextBox.IsEnabled = false;
            loginTextBox.IsEnabled = false;
            passwordBox.IsEnabled = false;
            Uzytkownik ja = channel.PobierzMojeDane();
            userInfoBlock.Text = $"{ja.Imie} {ja.Nazwisko}\nPrivileges: {ja.Uprawnienia}";

            typWidoku.IsEnabled = true;
            odswiezBtn.IsEnabled = true;

            if (ja.Uprawnienia <= Uprawnienia.Uzytkownik)
            {
                widokLicencje.IsEnabled = false;
                widokProgramy.IsEnabled = false;
                widokUzytkownicy.IsEnabled = false;
                if (typWidoku.SelectedItem.Equals(widokMojeLicencje) || typWidoku.SelectedItem.Equals(widokMojeProgramy))
                {
                    dodajNowy.IsEnabled = false;
                    usun.IsEnabled = false;
                    usunId.IsEnabled = false;
                }
            }
            else
            {
                widokLicencje.IsEnabled = true;
                widokProgramy.IsEnabled = true;
                widokUzytkownicy.IsEnabled = true;
                if (!typWidoku.SelectedItem.Equals(widokMojeLicencje) && !typWidoku.SelectedItem.Equals(widokMojeProgramy))
                {
                    dodajNowy.IsEnabled = true;
                    usun.IsEnabled = true;
                    usunId.IsEnabled = true;
                }
            }

            typWidoku.SelectionChanged += (sender, args) =>
            {
                odswiezDane();
                if (typWidoku.SelectedItem.Equals(widokMojeLicencje) || typWidoku.SelectedItem.Equals(widokMojeProgramy))
                {
                    dodajNowy.IsEnabled = false;
                    usun.IsEnabled = false;
                    usunId.IsEnabled = false;
                }
                else
                {
                    dodajNowy.IsEnabled = true;
                    usun.IsEnabled = true;
                    usunId.IsEnabled = true;
                }
            };
            odswiezDane();

        }

        private void Wylogowany()
        {
            conBtn.Content = "Connect";
            channel = null;
            addressTextBox.IsEnabled = true;
            loginTextBox.IsEnabled = true;
            passwordBox.IsEnabled = true;
            userInfoBlock.Text = "Login to see more user information.";

            typWidoku.IsEnabled = false;
            odswiezBtn.IsEnabled = false;
            typWidoku.SelectedIndex = 4;
            dodajNowy.IsEnabled = false;
            usun.IsEnabled = false;
            usunId.IsEnabled = false;
        }

        private void odswiezDane()
        {
            if (typWidoku.SelectedItem.Equals(widokProgramy))
            {
                Programy = (channel?.PobierzProgramy() ?? new Program[0]).ToList();
                dataGrid.ItemsSource = Programy;
            }
            else if (typWidoku.SelectedItem.Equals(widokLicencje))
            {
                Licencje = (channel?.PobierzLicencje(null) ?? new Licencja[0]).ToList();
                dataGrid.ItemsSource = Licencje;
            }
            else if (typWidoku.SelectedItem.Equals(widokUzytkownicy))
            {
                Uzytkownicy = (channel?.PobierzUzytkownikow() ?? new Uzytkownik[0]).ToList();
                dataGrid.ItemsSource = Uzytkownicy;
            }
            else if (typWidoku.SelectedItem.Equals(widokMojeLicencje))
            {
                Licencje = (channel?.PobierzMojeLicencje() ?? new Licencja[0]).ToList();
                dataGrid.ItemsSource = Licencje;
            }
            else if (typWidoku.SelectedItem.Equals(widokMojeProgramy))
            {
                Programy = (channel?.PobierzMojeProgramy() ?? new Program[0]).ToList();
                dataGrid.ItemsSource = Programy;
            }
            dataGrid.Columns.RemoveAt(0);
        }

        private void odswiezBtn_Click(object sender, RoutedEventArgs e)
        {
            odswiezDane();
        }

        private void dodajNowy_Click(object sender, RoutedEventArgs e)
        {
            if (typWidoku.SelectedItem.Equals(widokUzytkownicy))
            {
                OknoDodawaniaUzytkownika dodajUzytkownika = new OknoDodawaniaUzytkownika();
                dodajUzytkownika.ShowDialog();
                if (dodajUzytkownika.NowyUzytkownik != null)
                {
                    if (
                        channel.DodajUzytkownika(dodajUzytkownika.NowyUzytkownik.Imie,
                            dodajUzytkownika.NowyUzytkownik.Nazwisko, dodajUzytkownika.NowyUzytkownik.Login,
                            dodajUzytkownika.Haslo, dodajUzytkownika.NowyUzytkownik.Uprawnienia) ==
                        WiadomoscZwrotna.Pomyslnie)
                    {
                        odswiezDane();
                        MessageBox.Show("New user added successfully.", "Information", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show("Adding a new user failed.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
                
            }
            else if (typWidoku.SelectedItem.Equals(widokProgramy))
            {
                OknoDodawaniaProgramu dodajProgram = new OknoDodawaniaProgramu();
                dodajProgram.ShowDialog();
                if (dodajProgram.Nazwa != null)
                {
                    if (
                        channel.DodajProgram(dodajProgram.Nazwa) ==
                        WiadomoscZwrotna.Pomyslnie)
                    {
                        odswiezDane();
                        MessageBox.Show("New software added successfully.", "Information", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show("Adding a new software failed.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
                
            }
            else if (typWidoku.SelectedItem.Equals(widokLicencje))
            {
                OknoDodawaniaLicencji dodajLicencje = new OknoDodawaniaLicencji();
                dodajLicencje.uzytkownikBox.ItemsSource =
                    channel.PobierzUzytkownikow().Select(u => $"{u.Imie} {u.Nazwisko}");
                dodajLicencje.programBox.ItemsSource = channel.PobierzProgramy().Select(p => $"{p.Nazwa}");
                dodajLicencje.ShowDialog();
                if (dodajLicencje.IdProgramu != null && dodajLicencje.IdUzytkownika != null)
                {
                    if (channel.PobierzLicencje(dodajLicencje.IdUzytkownika) == null ||
                        channel.PobierzLicencje(dodajLicencje.IdUzytkownika)
                            .ToList()
                            .All(l => l.ProgramID != dodajLicencje.IdProgramu))
                    {
                        if (channel.DodajLicencje(dodajLicencje.IdUzytkownika.Value, dodajLicencje.IdProgramu.Value, dodajLicencje.data.SelectedDate.Value) == WiadomoscZwrotna.Pomyslnie)
                        {
                            odswiezDane();
                            MessageBox.Show("New license added successfully.", "Information", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                            MessageBox.Show("Adding a new license failed.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show("This user already owns license for this software.", "Warning",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
        }

        private void usun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = int.Parse(usunId.Text);
                UsuwanyTyp typ;

                if (typWidoku.SelectedItem.Equals(widokUzytkownicy)) typ = UsuwanyTyp.Uzytkownik;
                else if (typWidoku.SelectedItem.Equals(widokProgramy)) typ = UsuwanyTyp.Program;
                else if (typWidoku.SelectedItem.Equals(widokLicencje)) typ = UsuwanyTyp.Licencja;
                else throw new NotImplementedException();

                if (channel.UsunPozycje(id, typ) == WiadomoscZwrotna.Pomyslnie)
                {
                    odswiezDane();
                    MessageBox.Show("Removed successfully.", "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else MessageBox.Show("Removing failed.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("No ID or incorrect format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
