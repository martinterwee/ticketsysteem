using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSysteemMVC5.Models
{
    public class Status
    {
        public int Id { get; set; }
        [StringLength(64)]
        [Required]
        public string Naam { get; set; }
    }
}