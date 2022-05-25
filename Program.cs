using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Text.Json.Serialization;
using programming_task_1;


namespace IraTask_1;

using static IraTask_1.RegistrationCertificate;
using static IraTask_1.Collection<RegistrationCertificate>;


class Program
{
    public static void Main(string[] args)
    {
        // Menu<Freelancer> mainMenu = new Menu<Freelancer>("Freelancers.json");
        Menu<RegistrationCertificate> mainMenu = new Menu<RegistrationCertificate>("RegistrationCertificate.json");
        mainMenu.Run();
    }
    
    
}