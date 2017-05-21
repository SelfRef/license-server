using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace SerwerLicencji
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Licencje" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Licencje.svc or Licencje.svc.cs at the Solution Explorer and start debugging.
    public class Licencje : ILicencje
    {
        private Uprawnienia UprawnieniaUzytkownika { get; set; } = Uprawnienia.Anonim;
        private int? IdUzytkownika { get; set; }

        public WiadomoscZwrotna DodajLicencje(int uzytkownikId, int programId, DateTime dataWygasniecia)
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator && BazaDanych.Uzytkownicy.Any(u => u.ID == uzytkownikId) && BazaDanych.Programy.Any(p => p.ID == programId))
            {
                Licencja licencja = new Licencja() { DataWygasniecia = dataWygasniecia, ProgramID = programId };
                licencja.ID = BazaDanych.Licencje.LastOrDefault()?.ID + 1 ?? 0;

                BazaDanych.Licencje.Add(licencja);
                var uzytkownikLicencji = BazaDanych.Uzytkownicy.Single(u => u.ID == uzytkownikId);
                uzytkownikLicencji.LicencjeID.Add(licencja.ID);
                return WiadomoscZwrotna.Pomyslnie;
            }
            else return WiadomoscZwrotna.Niepomyslnie;
        }

        public WiadomoscZwrotna DodajProgram(string nazwa)
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator && BazaDanych.Programy.All(p => p.Nazwa != nazwa))
            {
                Program program = new Program() { Nazwa = nazwa };
                program.ID = BazaDanych.Programy.LastOrDefault()?.ID + 1 ?? 0;
                BazaDanych.Programy.Add(program);
                return WiadomoscZwrotna.Pomyslnie;
            }
            else return WiadomoscZwrotna.Niepomyslnie;
        }

        public WiadomoscZwrotna DodajUzytkownika(string imie, string nazwisko, string login, string haslo, Uprawnienia uprawnienia)
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator && BazaDanych.Uzytkownicy.All(u => u.Login != login))
            {
                Uzytkownik uzytkownik = new Uzytkownik() { Imie = imie, Nazwisko = nazwisko, Login = login, Haslo = CalculateMD5Hash(haslo), Uprawnienia = uprawnienia, LicencjeID = new BindingList<int>() };
                uzytkownik.ID = BazaDanych.Uzytkownicy.LastOrDefault()?.ID + 1 ?? 0;
                BazaDanych.Uzytkownicy.Add(uzytkownik);
                return WiadomoscZwrotna.Pomyslnie;
            }
            else return WiadomoscZwrotna.Niepomyslnie;
        }

        public WiadomoscZwrotna Logowanie(string login, string haslo)
        {
            string hash = CalculateMD5Hash(haslo);
            if (BazaDanych.Uzytkownicy.Any(u => u.Login == login && u.Haslo == CalculateMD5Hash(haslo)))
            {
                Uzytkownik uzytkownik = BazaDanych.Uzytkownicy.Single(u => u.Login == login && u.Haslo == CalculateMD5Hash(haslo));
                UprawnieniaUzytkownika = uzytkownik.Uprawnienia;
                IdUzytkownika = uzytkownik.ID;
                return WiadomoscZwrotna.Pomyslnie;
            }
            else return WiadomoscZwrotna.Niepomyslnie;
        }

        public List<Licencja> PobierzLicencje(int? uzytkownik = default(int?))
        {

            if (UprawnieniaUzytkownika == Uprawnienia.Administrator)
            {
                if (uzytkownik == null)
                {
                    return BazaDanych.Licencje.ToList();
                }
                else if (BazaDanych.Uzytkownicy.Any(u => u.ID == uzytkownik))
                {
                    return
                    BazaDanych.Licencje.Where(
                        l => BazaDanych.Uzytkownicy.Single(u => u.ID == uzytkownik).LicencjeID.Contains(l.ID)).ToList();
                }

            }
            return null;
        }

        public List<Licencja> PobierzMojeLicencje()
        {
            if (UprawnieniaUzytkownika >= Uprawnienia.Uzytkownik)
            {
                return
                    BazaDanych.Licencje.Where(
                        l => BazaDanych.Uzytkownicy.Single(u => u.ID == IdUzytkownika).LicencjeID.Contains(l.ID)).ToList();
            }
            return null;
        }

        public List<Program> PobierzProgramy()
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator)
            {
                return BazaDanych.Programy.ToList();
            }
            else return null;
        }



        public List<Uzytkownik> PobierzUzytkownikow()
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator)
            {
                return BazaDanych.Uzytkownicy.ToList();
            }
            else return null;
        }

        public List<Program> PobierzMojeProgramy()
        {
            if (UprawnieniaUzytkownika >= Uprawnienia.Uzytkownik)
            {
                BindingList<int> mojeLicencjeId = BazaDanych.Uzytkownicy.Single(u => u.ID == IdUzytkownika).LicencjeID;
                List<Licencja> mojeLicencje = BazaDanych.Licencje.Where(l => mojeLicencjeId.Contains(l.ID)).ToList();
                List<int> mojeProgramyId = mojeLicencje.Select(l => l.ProgramID).ToList();
                List<Program> mojeProgramy = BazaDanych.Programy.Where(p => mojeProgramyId.Contains(p.ID)).ToList();
                return mojeProgramy;
            }
            return null;
        }

        public Uzytkownik PobierzMojeDane()
        {
            if (UprawnieniaUzytkownika >= Uprawnienia.Uzytkownik)
            {
                return BazaDanych.Uzytkownicy.Single(u => u.ID == IdUzytkownika);
            }
            return null;
        }

        public WiadomoscZwrotna UsunPozycje(int id, UsuwanyTyp typ)
        {
            if (UprawnieniaUzytkownika == Uprawnienia.Administrator)
            {
                if (typ == UsuwanyTyp.Uzytkownik && BazaDanych.Uzytkownicy.Any(u => u.ID == id))
                {
                    BazaDanych.Uzytkownicy.Remove(BazaDanych.Uzytkownicy.Single(u => u.ID == id));
                    return WiadomoscZwrotna.Pomyslnie;
                }
                else if (typ == UsuwanyTyp.Program && BazaDanych.Programy.Any(u => u.ID == id))
                {
                    BazaDanych.Programy.Remove(BazaDanych.Programy.Single(u => u.ID == id));
                    return WiadomoscZwrotna.Pomyslnie;
                }
                else if (typ == UsuwanyTyp.Licencja && BazaDanych.Licencje.Any(u => u.ID == id))
                {
                    BazaDanych.Licencje.Remove(BazaDanych.Licencje.Single(u => u.ID == id));
                    return WiadomoscZwrotna.Pomyslnie;
                }
            }
            return WiadomoscZwrotna.Niepomyslnie;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    #region DATACONTRACTS
    [DataContract]
    public class Uzytkownik : INotifyPropertyChanged
    {
        public Uzytkownik()
        {
            LicencjeID.ListChanged +=
                (sender, args) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicencjeID"));
        }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Imie { get; set; }

        [DataMember]
        public string Nazwisko { get; set; }

        [DataMember]
        public BindingList<int> LicencjeID { get; set; } = new BindingList<int>();

        [DataMember]
        public string Login { get; set; }

        [JsonProperty]
        public string Haslo { get; set; }

        [DataMember]
        public Uprawnienia Uprawnienia { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [DataContract]
    public class Program
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Nazwa { get; set; }
    }

    [DataContract]
    public class Licencja
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public DateTime DataWygasniecia { get; set; }

        [DataMember]
        public int ProgramID { get; set; }
    }
    #endregion

    #region ENUMS
    [DataContract]
    public enum Uprawnienia
    {
        [EnumMember]
        Anonim,

        [EnumMember]
        Uzytkownik,

        [EnumMember]
        Administrator
    }

    [DataContract]
    public enum WiadomoscZwrotna
    {
        [EnumMember]
        Pomyslnie,

        [EnumMember]
        Niepomyslnie
    }

    [DataContract]
    public enum UsuwanyTyp
    {
        [EnumMember]
        Uzytkownik,

        [EnumMember]
        Program,

        [EnumMember]
        Licencja
    }
    #endregion

}
