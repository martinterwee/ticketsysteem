using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TicketSysteemMVC5.Models;

namespace TicketSysteemMVC5.Config
{
    public static class DefaultRolesAndUsers
    {
        internal const string PassWord = "Welkom123!";

       
        /// <summary>
        /// Maakt rollen aam die worden gebruikt in het Ticketsysteem.
        /// <para>uses=admin@ticketsysteem.nl met password=Welkom123! wordt aangemaakt</para> 
        /// </summary>
        /// <param name="db"></param>
        public static void Initialize(ApplicationDbContext db)
        {
//            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
//            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//
//            // Maak Administrator rol en Administrator aan
//            if (!roleManager.RoleExists(RoleNames.Administrator))
//            {
//                IdentityRole role = new IdentityRole { Name = RoleNames.Administrator };
//                roleManager.Create(role);
//
//                // voeg standaard Administrator toe
//                ApplicationUser user = new ApplicationUser
//                {
//                    Email = "admin@ticketssysteem.nl",
//                    Voornaam = "Admin",
//                    Tussenvoegsel = "de",
//                    Achternaam = "Strator",
//                    Telefoonnummer = "0612345678",
//                    ChangePassword = true
//                };
//                user.UserName = user.Email;
//
//                IdentityResult administrator = UserManager.Create(user, PassWord);
//
//                if (administrator.Succeeded)
//                {
//                    UserManager.AddToRole(user.Id, RoleNames.Administrator);
//                }
//            }
//
//            // Maak de rol Medewerker aan
//            if (!roleManager.RoleExists(RoleNames.Medewerker))
//            {
//                IdentityRole role = new IdentityRole { Name = RoleNames.Medewerker };
//                roleManager.Create(role);
//            }
//
//            // Maak de rol Klant aan
//            if (!roleManager.RoleExists(RoleNames.Klant))
//            {
//                IdentityRole role = new IdentityRole { Name = RoleNames.Klant };
//                roleManager.Create(role);
//            }
        }
    }
}