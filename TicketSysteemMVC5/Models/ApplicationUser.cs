using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TicketSysteemMVC5.Models
{
    /// <summary>
    /// Gebruiker van het systeem
    /// <para>Een gebruiker kan de rol "Medewerker" of "Klant" hebben</para>
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// De Voornaam van de gebruiker
        /// </summary>
        [StringLength(64)]
        [Required]
        public string Voornaam { get; set; }

        /// <summary>
        /// Het tussenvoegsel van de naam van de gebruiker
        /// </summary>
        [StringLength(32)]
        public string Tussenvoegsel{ get; set; }

        /// <summary>
        /// De Achternaam van de gebruiker
        /// </summary>
        [StringLength(64)]
        [Required]
        public string Achternaam { get; set; }

        /// <summary>
        /// Het telefoonnummer van de gebruiker
        /// </summary>
        [StringLength(11)]
        public string Telefoonnummer { get; set; }

        /// <summary>
        /// Automatisch aangemaakt accounts en Medewerkers moeten hun password veranderen bij eerste inlog
        /// </summary>
        public bool ChangePassword { get; set; } = false;

        /// <summary>
        /// De volledige naam van de gebruiker
        /// </summary>
        [NotMapped]
        public string Naam => $"{Voornaam} {Tussenvoegsel} {Achternaam}";

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /// <summary>
        /// Een gebruiker in de rol van Klant kan 0, of meer tickets hebben
        /// </summary>
        public virtual ICollection<Ticket> Tickets { get; set; }

        /// <summary>
        /// Een gebruiker in de rol van Medewerker kan 0 of meer Applicaties beheren
        /// </summary>
        public virtual ICollection<Applicatie> Applicaties { get; set; }
    }
}