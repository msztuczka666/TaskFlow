using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Imie { get; set; }
        public string? Nazwisko { get; set; }
        public string? Stanowisko { get; set; }
    }
    
    public static class Stanowiska
    {
        public const string Administrator = "Administrator";
        public const string Mistrz = "Mistrz";
        public const string Specjalista = "Specjalista";
        public const string KierownikBudowy = "Kierownik budowy";
        public const string KierownikDzialu = "Kierownik działu";
        public const string Gosc = "Gość";
        
        public static List<string> Wszystkie => new()
        {
            Administrator,
            Mistrz,
            Specjalista,
            KierownikBudowy,
            KierownikDzialu,
            Gosc
        };
    }
}
