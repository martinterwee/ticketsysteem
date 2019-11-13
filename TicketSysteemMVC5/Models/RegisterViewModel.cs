using System.ComponentModel.DataAnnotations;

namespace TicketSysteemMVC5.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [StringLength(64)]
        [Required]
        public string Voornaam { get; set; }
       
        [StringLength(32)]
        public string Tussenvoegsel { get; set; }

        [StringLength(64)]
        [Required]
        public string Achternaam { get; set; }

        [StringLength(11)]
        public string Telefoonnummer { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Het {0} moet minimaal {2} tekens lang zijn.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        [Compare("Password", ErrorMessage = "Het Wachtwoord en de bevestiging zijn niet identiek.")]
        public string ConfirmPassword { get; set; }
    }
}