using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace IraTask_1;

using static IraTask_1.RegistrationCertificate;
using static IraTask_1.Collection<RegistrationCertificate>;

using static IraTask_1.Certificate;


class Program
{
    public static void Main(string[] args)
    {
        // Menu<RegistrationCertificate> mainMenu = new Menu<RegistrationCertificate>("RegistrationCertificate.json");
        // Menu<VaccinationRequest> mainMenu = new Menu<VaccinationRequest>("test.json");
        // Menu<Contract> mainMenu = new Menu<Contract>("TestData.json");
        Menu<Certificate> mainMenu = new Menu<Certificate>("certificate.json");
        mainMenu.Run();
    }
    
    
}