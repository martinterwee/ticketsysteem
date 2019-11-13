using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketSysteemMVC5.Models
{
    /// <summary>
    /// Ticket voor Applicatiebeheer
    /// </summary>
    public class TicketViewModel
    {
        /// <summary>
        /// Id, primary key, Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Placeholder voor KlantId. Wordt niet handmatig ingevuld
        /// </summary>
        public string KlantId { get; set; }

        /// <summary>
        /// KLant is voor alleen voor display
        /// </summary>
        public ApplicationUser Klant { get; set; }

        /// <summary>
        /// De betreffende Applicatie
        /// </summary>
        [Required]
        public int ApplicatieId { get; set; }

        public List<Applicatie> Applicaties { get; set; }

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