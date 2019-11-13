using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TicketSysteemMVC5.Config;
using TicketSysteemMVC5.Models;

namespace TicketSysteemMVC5.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Navigation()
        {
            if (User.IsInRole(RoleNames.Administrator))
                return PartialView("_AdministratorNav");

            if (User.IsInRole(RoleNames.Medewerker))
                return PartialView("_MedewerkerNav");

            if (User.IsInRole(RoleNames.Klant))
                return PartialView("_KlantNav");

            return PartialView("_EmptyNav");
        }
    }
}