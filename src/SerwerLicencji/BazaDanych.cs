using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SerwerLicencji
{
    /// <summary>
    /// Wastwa bazy danych.
    /// </summary>
    public static class BazaDanych
    {
        private static string nazwaPlikuUzytkownikow = "users.json";
        private static string nazwaPlikuProgramow = "software.json";
        private static string nazwaPlikuLicencji = "licenses.json";

        /// <summary>
        /// Lista przechowująca obiekty reprezentujące użytkowników w bazie danych.
        /// </summary>
        public static BindingList<Uzytkownik> Uzytkownicy { get; set; }
        /// <summary>
        /// Lista przechowująca obiekty reprezentujące programy w bazie danych.
        /// </summary>
        public static BindingList<Program> Programy { get; set; }
        /// <summary>
        /// Lista przechowująca obiekty reprezentujące licencje w bazie danych.
        /// </summary>
        public static BindingList<Licencja> Licencje { get; set; }

        static BazaDanych()
        {
            if (!File.Exists(nazwaPlikuUzytkownikow)) File.Create(nazwaPlikuUzytkownikow);
            if (!File.Exists(nazwaPlikuProgramow)) File.Create(nazwaPlikuProgramow);
            if (!File.Exists(nazwaPlikuLicencji)) File.Create(nazwaPlikuLicencji);

            Uzytkownicy = new BindingList<Uzytkownik>(JsonConvert.DeserializeObject<List<Uzytkownik>>(File.ReadAllText(nazwaPlikuUzytkownikow)) ?? new List<Uzytkownik>());
            Programy = new BindingList<Program>(JsonConvert.DeserializeObject<List<Program>>(File.ReadAllText(nazwaPlikuProgramow)) ?? new List<Program>());
            Licencje = new BindingList<Licencja>(JsonConvert.DeserializeObject<List<Licencja>>(File.ReadAllText(nazwaPlikuLicencji)) ?? new List<Licencja>());

            Uzytkownicy.ListChanged +=
                (sender, args) => File.WriteAllText(nazwaPlikuUzytkownikow, JsonConvert.SerializeObject(Uzytkownicy));
            Programy.ListChanged +=
                (sender, args) => File.WriteAllText(nazwaPlikuProgramow, JsonConvert.SerializeObject(Programy));
            Licencje.ListChanged +=
                (sender, args) => File.WriteAllText(nazwaPlikuLicencji, JsonConvert.SerializeObject(Licencje));
        }

        
    }
}