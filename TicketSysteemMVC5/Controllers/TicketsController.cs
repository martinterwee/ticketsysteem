using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TicketSysteemMVC5.Config;
using TicketSysteemMVC5.Models;


namespace TicketSysteemMVC5.Controllers
{

    [Authorize(Roles = RoleNames.BewerkTickets)]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUser CurrentUser
        {
            get
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                return manager.FindById(User.Identity.GetUserId());
            }
        }

        /// <summary>
        /// Lijst van alle Tickets
        /// <para>Administrator ziet alle tickets</para>
        /// <para>Medewerker ziet alle tickets van de Applicaties die hij beheert</para>
        /// <para>Klant ziet alle Tickets die hij heeft ingevoerd</para>
        /// </summary>
        /// <param name="id">Id van de gebruiker</param>
        /// <returns></returns>
        [NonAction]
        public List<Ticket> TicketList(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                id = User.Identity.GetUserId();
            }

            ApplicationUser gebruiker = db.Users.Find(id);

            if (gebruiker == null)
            {
                return new List<Ticket>();
            }

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roles = manager.GetRoles(gebruiker.Id);

            // Administrator ziet alle tickets
            var selectie = db.Tickets
                .Include(t => t.Applicatie)
                .Include(t => t.Klant)
                .Include(t => t.Applicatie.Beheerder);

            // Medewerker ziet alle open selectie van de Applicaties die hij beheert
            if (roles.Contains(RoleNames.Medewerker))
            {
                selectie = selectie
                    .Where(t => t.Applicatie.Beheerder.Id == gebruiker.Id && t.Status != TicketStatus.Gesloten);

                var applicaties = db.Applicaties
                    .Where(a => a.Beheerder.Id == gebruiker.Id);
            }

            // Klant ziet alle Tickets die hij heeft ingevoerd
            if (roles.Contains(RoleNames.Klant))
            {
                selectie = selectie
                    .Where(t => t.Klant.Id == gebruiker.Id
                                && t.Status != TicketStatus.Gesloten);
            }

            var tickets = selectie.OrderBy(t => t.Status).ThenBy(t => t.Datum);

            return tickets.ToList();
        }

        // GET: Tickets
        /// <summary>
        /// Lijst van Tickets
        /// </summary>
        /// <returns>View met Lijst met Tickets</returns>
        public ActionResult Index()
        {
            List<Ticket> ticketList = TicketList("");
            
            return View(ticketList);
        }

        // GET: Tickets/Details/5
        /// <summary>
        /// Geselecteerde Ticket
        /// <para>Administrator ziet alle Tickets</para>
        /// <para>Medewerker mag alleen tickets zien van de Applicaties die hij beheert</para>
        /// <para>Klant mag alleen tickets waar hij als klant aan is gekoppeld</para>
        /// </summary>
        /// <param name="id">Id van de Ticket</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);



            if (ticket == null)
            {
                return HttpNotFound();
            }

            db.Entry(ticket).Reference(t => t.Applicatie).Load();
            db.Entry(ticket).Reference(t => t.Klant).Load();
            Applicatie applicatie = ticket.Applicatie;
            db.Entry(applicatie).Reference(t => t.Beheerder).Load();


            // Medewerker mag alleen tickets zien van de Applicaties die hij beheert
            if (User.IsInRole(RoleNames.Medewerker))
            {
                if (ticket.Applicatie.Beheerder.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Klant mag alleen tickets waar hij als klant aan is gekoppeld
            if (User.IsInRole(RoleNames.Klant))
            {
                if (ticket.Klant.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        /// <summary>
        /// Klant wil een nieuwe Ticket maken
        /// </summary>
        /// <returns>View om nieuwe ticket te maken</returns>
        /// <param name="id">(Optioneel) De id van de Applicatie war deze Ticket voor wordt gemaakt</param>
        [Authorize(Roles=RoleNames.CreeerTickets)]
        public ActionResult Create(int? id)
        {
            int applicatieId = 0;
            if (id != null)
            {
                Applicatie applicatie = db.Applicaties.Find(id);
                applicatieId = applicatie.Id;
            }
            TicketViewModel ticketView = new TicketViewModel
            {
                ApplicatieId = applicatieId,
                Applicaties = db.Applicaties.ToList()
            };
            
            return View(ticketView);
        }

        // POST: Tickets/Create
        /// <summary>
        /// Maakt een Ticket van een TicketViewModel en vult als Klant de huidige User in
        /// </summary>
        /// <param name="ticketView">Het ViewModel voor Tickets</param>
        /// <returns>Lijst met Tickets indien gelukt, anders View om Ticket te maken</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.CreeerTickets)]
        public ActionResult Create([Bind(Include = "Id,ApplicatieId,Onderwerp,Omschrijving")] TicketViewModel ticketView)
        {
            if (ModelState.IsValid)
            {
                Ticket ticket = new Ticket
                {
                    Id = ticketView.Id,
                    Klant = CurrentUser,
                    Applicatie = db.Applicaties.Find(ticketView.ApplicatieId),
                    Datum = DateTime.Today,
                    Onderwerp = ticketView.Onderwerp,
                    Omschrijving = ticketView.Omschrijving
                };
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketView);
        }

        // GET: Tickets/Edit/5
        /// <summary>
        /// Gebruik een View om een Ticket te bewerken
        /// </summary>
        /// <param name="id">Id van de Ticket</param>
        /// <returns>View om Ticket te bewerken</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            db.Entry(ticket).Reference(t => t.Applicatie).Load();
            db.Entry(ticket).Reference(t => t.Klant).Load();
            Applicatie applicatie = ticket.Applicatie;
            db.Entry(applicatie).Reference(t => t.Beheerder).Load();

            // Medewerker mag alleen tickets zien van de Applicaties die hij beheert
            if (User.IsInRole(RoleNames.Medewerker))
            {
                if (ticket.Applicatie.Beheerder.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Klant mag alleen tickets waar hij als klant aan is gekoppeld
            if (User.IsInRole(RoleNames.Klant))
            {
                if (ticket.Klant.Id!= CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ticket editTicket)
        {
            Ticket ticket = db.Tickets.Find(editTicket.Id);

            if (ticket != null)
            {
                ticket.Omschrijving = editTicket.Omschrijving;

                if (editTicket.Status == TicketStatus.Nieuw)
                    editTicket.Status = TicketStatus.InBehandeling;

                ticket.Status = editTicket.Status;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            db.Entry(ticket).Reference(t => t.Applicatie).Load();
            db.Entry(ticket).Reference(t => t.Klant).Load();
            Applicatie applicatie = ticket.Applicatie;
            db.Entry(applicatie).Reference(t => t.Beheerder).Load();

            // Medewerker mag alleen tickets zien van de Applicaties die hij beheert
            if (User.IsInRole(RoleNames.Medewerker))
            {
                if (ticket.Applicatie.Beheerder.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Klant mag alleen tickets waar hij als klant aan is gekoppeld
            if (User.IsInRole(RoleNames.Klant))
            {
                if (ticket.Klant.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);

            // Medewerker mag alleen tickets zien van de Applicaties die hij beheert
            if (User.IsInRole(RoleNames.Medewerker))
            {
                if (ticket.Applicatie.Beheerder.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Klant mag alleen tickets waar hij als klant aan is gekoppeld
            if (User.IsInRole(RoleNames.Klant))
            {
                if (ticket.Klant.Id != CurrentUser.Id)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            ticket.Status = TicketStatus.Gesloten;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
