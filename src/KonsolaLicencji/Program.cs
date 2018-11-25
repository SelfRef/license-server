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
                Console.WriteLine("Current server state: " + host.State.ToString());
                Console.WriteLine("Listening on the address: " + host.BaseAddresses.First().AbsoluteUri);
                if (host.State != CommunicationState.Opened)
                {
                    try
                    {
                        host.Open();
                        Console.WriteLine("Server is running");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: " + e.Message);
                    }
                }
                else Console.WriteLine("Server is already running");
                Console.WriteLine("Current server state: " + host.State.ToString());
                WyswietlPolecenia();
                if (host.State == CommunicationState.Opened)
                {
                    while (true)
                    {
                        switch (Console.ReadLine())
                        {
                            case "/?":
                                WyswietlPolecenia();
                                break;
                            case "/addadmin":
                                DodajUzytkownika(Uprawnienia.Administrator);
                                break;
                            case "/adduser":
                                DodajUzytkownika(Uprawnienia.Uzytkownik);
                                break;
                            case "/listallusers":
                                WyswietlUzytkownikow();
                                break;
                            case "/q":
                                host.Close();
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("Unknown command");
                                break;
                        }
                    }
                }
            }
        }

        private static void WyswietlPolecenia()
        {
            Console.WriteLine("Type command:\n" +
                              "/addadmin - add new administrator to database\n" +
                              "/adduser - add new user to database\n" +
                              "/listallusers - print list of all users in database\n" +
                              "/q - quit server\n" +
                              "/? - print list of commands again");
        }

        private static void DodajUzytkownika(Uprawnienia uprawnienia)
        {
            Uzytkownik uzytkownik = new Uzytkownik();
            Console.Write("Firstname: ");
            uzytkownik.Imie = Console.ReadLine();
            Console.Write("Lastname: ");
            uzytkownik.Nazwisko = Console.ReadLine();
            Console.Write("Login: ");
            uzytkownik.Login = Console.ReadLine();
            Console.Write("Password: ");
            uzytkownik.Haslo = Md5.CalculateMD5Hash(Console.ReadLine());
            uzytkownik.LicencjeID = new BindingList<int>();
            uzytkownik.Uprawnienia = uprawnienia;
            uzytkownik.ID = BazaDanych.Uzytkownicy.LastOrDefault()?.ID + 1 ?? 0;
            BazaDanych.Uzytkownicy.Add(uzytkownik);
            Console.WriteLine("Successfully added a new user");
        }

        private static void WyswietlUzytkownikow()
        {
            Console.WriteLine("User list:");
            if (BazaDanych.Uzytkownicy.Count == 0)
            {
                Console.WriteLine("<list is empty>");
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
