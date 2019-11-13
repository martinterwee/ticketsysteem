using System.ComponentModel.DataAnnotations;

namespace TicketSysteemMVC5.Models
{
    /// <summary>
    /// Een Applicatie die wordt beheerd
    /// </summary>
    public class Applicatie
    {
        /// <summary>
        /// Id, primary key, Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// De naam van de Applicatie
        /// </summary>
        [StringLength(64)]
        [Required]
        public string Naam { get; set; }

        /// <summary>
        /// De gebruiker die de Applicatie beheert
        /// </summary>
        public ApplicationUser Beheerder { get; set; }
    }
}