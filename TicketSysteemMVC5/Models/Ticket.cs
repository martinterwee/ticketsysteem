using System;
using System.ComponentModel.DataAnnotations;

namespace TicketSysteemMVC5.Models
{
    public enum TicketStatus
    {
        
        Nieuw,
        [Display(Name="In behandeling")]
        InBehandeling,
        Gesloten
    }

    /// <summary>
    /// Ticket voor Applicatiebeheer
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Id, primary key, Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// De Klant die de Ticket inlegt.
        /// <para>Wordt automatisch toegevoegd aan de hand van login</para>
        /// </summary>
        [Required]
        public ApplicationUser Klant { get; set; }

        /// <summary>
        /// De betreffende Applicatie
        /// </summary>
        [Required]
        public Applicatie Applicatie { get; set; }

        /// <summary>
        /// De datum waarop de ticket is gemaakt.
        /// <para>Wordt Automatisch toegevoegd</para>
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime Datum { get; set; }

        /// <summary>
        /// Het onderwerp van de Ticket
        /// </summary>
        [StringLength(128)]
        [Required]
        public string Onderwerp { get; set;  }

        /// <summary>
        /// De omschrijving van de Ticket
        /// </summary>
        [StringLength(512)]
        public string Omschrijving { get; set; }

        /// <summary>
        /// Status van de Ticket
        /// <para>Standaardwaarde is "Nieuw</para>
        /// </summary>
        public TicketStatus Status { get; set; } = TicketStatus.Nieuw;
    }
}