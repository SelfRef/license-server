using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SerwerLicencji
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILicencje" in both code and config file together.
    [ServiceContract]
    public interface ILicencje
    {

        [OperationContract]
        WiadomoscZwrotna Logowanie(string login, string haslo);

        [OperationContract]
        List<Licencja> PobierzMojeLicencje();

        [OperationContract]
        List<Uzytkownik> PobierzUzytkownikow();

        [OperationContract]
        Uzytkownik PobierzMojeDane();

        [OperationContract]
        List<Licencja> PobierzLicencje(int? uzytkownik = null);

        [OperationContract]
        List<Program> PobierzProgramy();

        [OperationContract]
        List<Program> PobierzMojeProgramy();

        [OperationContract]
        WiadomoscZwrotna DodajUzytkownika(string imie, string nazwisko, string login, string haslo, Uprawnienia uprawnienia);

        [OperationContract]
        WiadomoscZwrotna DodajProgram(string nazwa);

        [OperationContract]
        WiadomoscZwrotna DodajLicencje(int uzytkownikId, int programId, DateTime dataWygasniecia);

        [OperationContract]
        WiadomoscZwrotna UsunPozycje(int id, UsuwanyTyp typ);

        // TODO: Add your service operations here
    }

}
