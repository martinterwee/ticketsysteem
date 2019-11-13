using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TicketSysteemMVC5.Config;
using TicketSysteemMVC5.Models;

namespace TicketSysteemMVC5.Controllers
{
    public class ApplicatiesController : Controller
    {
        /// <summary>
        /// Geeft de huidige gebruiker als ApplicationUser object
        /// </summary>
        private ApplicationUser CurrentUser
        {
            get
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                return manager.FindById(User.Identity.GetUserId());
            }
        }

        /// <summary>
        /// Geeft een lijst van gebruikers met de rol "Medewerker"
        /// </summary>
        private List<ApplicationUser> Medewerkers
        {
            get
            {
                IdentityRole theRole = db.Roles
                    .FirstOrDefault(r => r.Name.Equals(RoleNames.Medewerker));

                List<ApplicationUser> medewerkers = (db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == theRole.Id)))
                    .ToList();

                return medewerkers;
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Applicatie> BeheerderApplicaties(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                id = User.Identity.GetUserId();
            }

            ApplicationUser gebruiker = db.Users.Find(id);

            if (gebruiker == null)
            {
                return null;
            }

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roles = manager.GetRoles(gebruiker.Id);

            // Klanten beheren geen Applicaties
            if (roles.Contains(RoleNames.Klant))
            {
                return null;
            }

            List<Applicatie> applicaties = db.Applicaties
                .Include(a => a.Beheerder)
                .ToList();

            if (roles.Contains(RoleNames.Medewerker))
            {
                applicaties = db.Applicaties
                    .Include(a => a.Beheerder)
                    .Where(a => a.Beheerder.Id == gebruiker.Id)
                    .ToList();
            }

            return applicaties;
        }

        // GET: Applicaties
        /// <summary>
        /// Toont een lijst van alle Applicaties inclusief beheerders
        /// </summary>
        /// <returns>View met Lijst van Applicaties</returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Index()
        {
            List<Applicatie> applicaties = db.Applicaties
                .Include(a => a.Beheerder)
                .ToList();

            if (User.IsInRole(RoleNames.Medewerker))
            {
                applicaties = db.Applicaties
                    .Include(a => a.Beheerder)
                    .Where(a => a.Beheerder.Id == CurrentUser.Id)
                    .ToList();
            }
            return View(applicaties);
        }

        // GET: Applicaties/Details/5
        /// <summary>
        /// Toont de gegevens van een Applicatie, inclusief de beheerder
        /// </summary>
        /// <param name="id">Het Id van de Applicatie</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicatie applicatie = db.Applicaties.Find(id);
            if (applicatie == null)
            {
                return HttpNotFound();
            }

            db.Entry(applicatie).Reference(a => a.Beheerder).Load();

            if (User.IsInRole(RoleNames.Medewerker))
            {
                if (applicatie.Beheerder.Id != CurrentUser.Id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                    
            }

            return View(applicatie);
        }

        // GET: Applicaties/Create
        /// <summary>
        /// Maakt een View om een Applicatie aan te maken.
        /// Gebruikt ApplicatieVIewModel mey een lijst van gebruikers om een beheerder te kiezen,
        /// </summary>
        /// <returns>View met invulscherm/returns>
        [Authorize(Roles = RoleNames.BewerkApplicaties)]
        public ActionResult Create()
        {
            ApplicatieViewModel applicatieView = new ApplicatieViewModel {Medewerkers = Medewerkers};
            return View(applicatieView);
        }

        // POST: Applicaties/Create
        /// <summary>
        /// Slaat de gegevens van ApplicatieViewModel op ijn een Applicatie
        /// </summary>
        /// <param name="applicatieView">object met gegevens over Applicatie/param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Naam,BeheerderId")] ApplicatieViewModel applicatieView)
        {
            if (ModelState.IsValid)
            {
                if (applicatieView.BeheerderId == null)
                {
                    applicatieView.BeheerderId = CurrentUser.Id;
                }

                Applicatie applicatie = new Applicatie
                {
                    Id = applicatieView.Id,
                    Naam = applicatieView.Naam,
                    Beheerder = db.Users.Find(applicatieView.BeheerderId)
                };
                db.Applicaties.Add(applicatie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicatieView);
        }

        // GET: Applicaties/Edit/5
        /// <summary>
        /// Maakt een View om een Applicatie te wijzigen
        /// </summary>
        /// <param name="id">Id van de Applicatie</param>
        /// <returns></returns>
        [Authorize(Roles = RoleNames.BewerkApplicaties)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicatie applicatie = db.Applicaties.Find(id);
            if (applicatie == null)
            {
                return HttpNotFound();
            }

            db.Entry(applicatie).Reference(a => a.Beheerder).Load();

            ApplicatieViewModel applicatieView = new ApplicatieViewModel
            {
                Id = applicatie.Id,
                Naam = applicatie.Naam,
                BeheerderId = applicatie.Beheerder.Id,
                Medewerkers = Medewerkers
            };
            return View(applicatieView);
        }

        // POST: Applicaties/Edit/5
        /// <summary>
        /// Wijzigt de Applicatie op basis van de gegevens die zijn ingevuld in het ApplicationViewModel
        /// </summary>
        /// <param name="applicatieView">De gegevens uit de View</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.BewerkApplicaties)]
        public ActionResult Edit([Bind(Include = "Id,Naam,BeheerderId")] ApplicatieViewModel applicatieView)
        {
            if (ModelState.IsValid)
            {
                Applicatie applicatie = db.Applicaties.Find(applicatieView.Id);
                db.Entry(applicatie).Reference(a => a.Beheerder).Load();

                ApplicationUser beheerder = db.Users.Find(applicatieView.BeheerderId);
               
                applicatie.Naam = applicatieView.Naam;
                applicatie.Beheerder = beheerder;
                
                db.Entry(applicatie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicatieView);
        }

        // GET: Applicaties/Delete/5
        [Authorize(Roles = RoleNames.BewerkApplicaties)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicatie applicatie = db.Applicaties.Find(id);
            if (applicatie == null)
            {
                return HttpNotFound();
            }
            return View(applicatie);
        }

        // POST: Applicaties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.BewerkApplicaties)]
        public ActionResult DeleteConfirmed(int id)
        {
            Applicatie applicatie = db.Applicaties.Find(id);
            db.Applicaties.Remove(applicatie);
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
