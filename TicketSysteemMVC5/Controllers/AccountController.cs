using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TicketSysteemMVC5.Config;
using TicketSysteemMVC5.Models;

namespace TicketSysteemMVC5.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        #endregion


        #region Administrator


        // GET /Account/
        /// <summary>
        /// Laat alle gebruikers van het systeem zien
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        /// <summary>
        /// Helper functie om alle gebruikers in een rol te laten zien
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>View met alle gebruikers</returns>
        private List<ApplicationUser> Gebruikers(string roleName)
        {
            IdentityRole theRole = db.Roles
                .FirstOrDefault(r => r.Name.Equals(roleName));

            List<ApplicationUser> gebruikers = (db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == theRole.Id)))
                .ToList();

            return gebruikers;
        }

        /// <summary>
        /// Laat alle Klanten zien
        /// </summary>
        /// <returns>View met alle Klanten</returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Klanten()
        {
            return View(Gebruikers(RoleNames.Klant));
        }

        /// <summary>
        /// Laat alle Medewerkers zien
        /// </summary>
        /// <returns>View met alle Medewerkers</returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Medewerkers()
        {
            return View(Gebruikers(RoleNames.Medewerker));
        }

        /// <summary>
        /// Laat de gegevens van één bepaalde gebruiker zien
        /// </summary>
        /// <param name="id">Id van de Gebruiker (GUID)</param>
        /// <returns>View met gegevens van gebruiker</returns>
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser gebruiker = db.Users.Find(id);

            if (gebruiker == null)
            {
                return HttpNotFound();
            }

            return View(gebruiker);
        }

        /// <summary>
        /// Maakt View om nieuwe Medewerker aan te maken
        /// </summary>
        /// <returns>View met MedewerkerviewModel</returns>
        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// Administrator maakt een nieuwe medewerker aan </summary>
        /// <param name="medewerker"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Create(MedewerkerViewModel medewerker)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = medewerker.Email,
                    Email = medewerker.Email,
                    Voornaam = medewerker.Voornaam,
                    Tussenvoegsel = medewerker.Tussenvoegsel,
                    Achternaam = medewerker.Achternaam,
                    Telefoonnummer = medewerker.Telefoonnummer,
                    ChangePassword = true
                };
                var result = UserManager.Create(user, DefaultRolesAndUsers.PassWord);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, RoleNames.Medewerker);

                    return RedirectToAction("Medewerkers");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(medewerker);
        }

        // GET Account/Edit/e814476b-ebb4-47e6-bba8-703cb6bafef0
        /// <summary>
        /// Geeft de mogelijkheid voor Administrators om gebruikersgegevens aan te passen
        /// </summary>
        /// <param name="id">User.Id (GUID)</param>
        /// <returns>View om gegevens aan te passen</returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser gebruiker = db.Users.Find(id);

            if (gebruiker == null)
            {
                return HttpNotFound();
            }

            GebruikerViewModel gebruikerView = new GebruikerViewModel
            {
                Id = gebruiker.Id,
                Email = gebruiker.Email,
                Voornaam = gebruiker.Voornaam,
                Tussenvoegsel = gebruiker.Tussenvoegsel,
                Achternaam = gebruiker.Achternaam,
                Telefoonnummer = gebruiker.Telefoonnummer
            };

            return View(gebruikerView);
        }

        /// <summary>
        /// Wijzigt gegevens van een gebruiker
        /// </summary>
        /// <param name="gebruikerView"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPost]
        public ActionResult Edit(GebruikerViewModel gebruikerView)
        {
            ApplicationUser gebruiker = db.Users.Find(gebruikerView.Id);

            if (gebruiker == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(gebruikerView);
            }

            gebruiker.Voornaam = gebruikerView.Voornaam;
            gebruiker.Tussenvoegsel = gebruikerView.Tussenvoegsel;
            gebruiker.Achternaam = gebruikerView.Achternaam;
            gebruiker.Telefoonnummer = gebruikerView.Telefoonnummer;

            db.Entry(gebruiker).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Toont het profiel van de gebruiker
        /// <para>Voor Klanten worden alle ingelegde Tickets getoond</para>
        /// <para>Voor Medewerkers worden alle Tickets en alle Applicaties getoond die de Medewerker beheert</para>
        /// </summary>
        /// <param name="id">Gebruikers Id</param>
        /// <returns>View met Profielinformatie</returns>
        public ActionResult Profiel(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                id = User.Identity.GetUserId();
            }

            ApplicationUser gebruiker = db.Users.Find(id);

            if (gebruiker == null)
            {
                return HttpNotFound();
            }

            return View(gebruiker);
        }

        /// <summary>
        /// PartialView met Lijst van alle Tickets
        /// <para>Administrator ziet alle tickets</para>
        /// <para>Medewerker ziet alle tickets van de Applicaties die hij beheert</para>
        /// <para>Klant ziet alle Tickets die hij heeft ingevoerd</para>
        /// </summary>
        /// <returns>PartialView met Lijst met Tickets</returns>
        public ActionResult GebruikerTickets(string id)
        {
            TicketsController ticketsController = new TicketsController();
            List<Ticket> tickets = ticketsController.TicketList(id);
            ticketsController.Dispose();

            return PartialView("_TicketList", tickets);
        }

        /// <summary>
        /// Toont een lijst van alle Applicaties die worden beheerd door gebruiker
        /// <para>Administrator ziet alle Applicaties</para>
        /// <para>Klant krijgt een Empty View</para>
        /// </summary>
        /// <returns>PartialView met Lijst van Applicaties</returns>
        public ActionResult BeheerderApplicaties(string id)
        {
            ApplicatiesController applicatiesController = new ApplicatiesController();
            List<Applicatie> applicaties = applicatiesController.BeheerderApplicaties(id);
            applicatiesController.Dispose();

            if (applicaties == null)
            {
                return new EmptyResult();
            }

            return PartialView("_ApplicatieList", applicaties);
        }

        #endregion

        #region Scaffolded
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        /// <summary>
        /// Login
        /// <para>Als Gebruiker is ingelogd en er is geen ReturnUrl, dan wordt hij omgeleid naar zijn Profielpagina</para>
        /// <para>Als user.ChangePassword, wordt de gebruiker omgeleid om het wachtwoord te veranderen</para>
        /// </summary>
        /// <param name="model">LoginViewModel met E-mail, Wachtwoord</param>
        /// <param name="returnUrl">De URL waar de gebruiker toegang toe wil</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var currentUser = await UserManager.FindByNameAsync(model.Email);
                    if (currentUser != null && currentUser.ChangePassword)
                    {
                        return RedirectToAction("ChangePassword", "Manage");
                    }

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Profiel");
                    }

                    return RedirectToLocal(returnUrl);
                default:
                    ModelState.AddModelError("", "U voerde ongeldige Login-gegevens in.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Voornaam = model.Voornaam,
                    Tussenvoegsel = model.Tussenvoegsel,
                    Achternaam = model.Achternaam,
                    Telefoonnummer = model.Telefoonnummer
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, RoleNames.Klant);

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }




        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


    }
}