using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using SerwerLicencji;

namespace KonsolaLicencji
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Licencje)))
            {
                Console.WriteLine("Aktualny status serwera: " + host.State.ToString());
                Console.WriteLine("Rozpoczynam nasłuchiwanie na adresie: " + host.BaseAddresses.First().AbsoluteUri);
                if (host.State != CommunicationState.Opened)
                {
                    try
                    {
                        host.Open();
                        Console.WriteLine("Serwer został uruchomiony pomyślnie");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Napotkano błąd, treść: " + e.Message);
                    }
                }
                else Console.WriteLine("Serwer jest już uruchomiony");
                Console.WriteLine("Aktualny status serwera: " + host.State.ToString());
                WyswietlPolecenia();
                if (host.State == CommunicationState.Opened)
                {
                    while (true)
                    {
                        switch (Console.ReadLine())
                        {
                            case "/p":
                                WyswietlPolecenia();
                                break;
                            case "/da":
                                DodajUzytkownika(Uprawnienia.Administrator);
                                break;
                            case "/du":
                                DodajUzytkownika(Uprawnienia.Uzytkownik);
                                break;
                            case "/wu":
                                WyswietlUzytkownikow();
                                break;
                            case "/w":
                                host.Close();
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("Nieznane polecenie");
                                break;
                        }
                    }
                }
            }
        }

        private static void WyswietlPolecenia()
        {
            Console.WriteLine("Wybierz opcję:\n" +
                              "/p - aby uzystkać listę wszystkich poleceń\n" +
                              "/da - aby dodać nowego administratora do bazy danych\n" +
                              "/du - aby dodać nowego użytkownika do bazy danych\n" +
                              "/wu = aby wyświetlić listę wszystkich użytkowników\n" +
                              "/w - aby zamknąć serwer i wyjść z aplikacji");
        }

        private static void DodajUzytkownika(Uprawnienia uprawnienia)
        {
            Uzytkownik uzytkownik = new Uzytkownik();
            Console.Write("Imię: ");
            uzytkownik.Imie = Console.ReadLine();
            Console.Write("Nazwisko: ");
            uzytkownik.Nazwisko = Console.ReadLine();
            Console.Write("Login: ");
            uzytkownik.Login = Console.ReadLine();
            Console.Write("Hasło: ");
            uzytkownik.Haslo = Console.ReadLine();
            uzytkownik.LicencjeID = new BindingList<int>();
            uzytkownik.Uprawnienia = uprawnienia;
            uzytkownik.ID = BazaDanych.Uzytkownicy.LastOrDefault()?.ID + 1 ?? 0;
            BazaDanych.Uzytkownicy.Add(uzytkownik);
            Console.WriteLine("Pomyślnie dodano nowego administratora");
        }

        private static void WyswietlUzytkownikow()
        {
            Console.WriteLine("Lista użytkowników:");
            if (BazaDanych.Uzytkownicy.Count == 0)
            {
                Console.WriteLine("Lista użytkowników jest pusta");
            }
            else
            {
                foreach (Uzytkownik u in BazaDanych.Uzytkownicy)
                {
                    Console.WriteLine($"{u.ID}. {u.Imie} {u.Nazwisko} - {u.Uprawnienia}");
                }
            }
        }
    }
}
