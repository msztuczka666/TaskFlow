using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Imie { get; set; }
        public string? Nazwisko { get; set; }
    }
}
