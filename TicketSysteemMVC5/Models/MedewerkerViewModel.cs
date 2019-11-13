using System.ComponentModel.DataAnnotations;

namespace TicketSysteemMVC5.Models
{
    public class MedewerkerViewModel
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
    }
}