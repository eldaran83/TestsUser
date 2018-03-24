using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using TestUsers.Data;
using TestUsers.Models;
using TestUsers.Models.Interfaces.Interfaces;
using TestUsers.Models.Repositories.IRepositories;
using TestUsers.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestUsers.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly IUtilisateurManager _utilisateurManager;
        private readonly IRepositoryFichier _fichierRepository;
        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="emailSender"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="context"></param>
        /// <param name="utilisateurRepository"></param>
        /// <param name="fichierRepository"></param>
        public AdminController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IHostingEnvironment env,
            ApplicationDbContext context,
            IUtilisateurManager utilisateurManager,
            IRepositoryFichier fichierRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _env = env;
            _context = context;
            _utilisateurManager = utilisateurManager;
            _fichierRepository = fichierRepository;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
                       //pas mieux comme idée pour gérer l'utilisateur eldaran83 pour le moment 
            //je recupere la vraie identité de l user qui doit etre l'admin la premiere fois 
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            string pseudoSuperAdmin = "Eldaran83";
            if (!await _utilisateurManager.PseudoExist(pseudoSuperAdmin)) //si le pseudo de l admin n'existe pas c est que l utilisateur n a pas été deja créée
            {
                Utilisateur adminUtilisateur = new Utilisateur
                {
                    //je récupere son id
                    ApplicationUserID = ApplicationUser,
                    ConfirmEmail = true,
                    DateCreationUtilisateur = DateTime.Now,
                    DateDeNaissance = DateTime.Now,
                    Email = "eldaran83@gmail.com",
                    ProfilUtilisateurComplet = true,
                    Pseudo = "Eldaran83",
                    Role = "Admin",
                    UrlAvatarImage = "/images/userDefault.png"

                };
                await _utilisateurManager.AddUtilisateurAdmin(adminUtilisateur);
            }
            //renvoyer la liste des utilisateurs
            IEnumerable<Utilisateur> listeUtilisateurs = await _utilisateurManager.GetAllUtilisateursAsync();
            return View(listeUtilisateurs);
        }

        /// <summary>
        /// GET edit 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            if (userId == null) // vérifie qu un id est bien passé en param
            {
                return NotFound();
            }
            //cherche le user dans la base 
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            if (utilisateur == null) //si on en trouve pas
            {
                return NotFound();
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            //      GESTION de la photo
            //////////////////////////////////////////////////////////////////////////////////////////
            if (utilisateur.UrlAvatarImage != null)
            {
                string img = utilisateur.UrlAvatarImage.ToString();
                ViewBag.ImgPath = img;
            }
            else
            {
                ViewBag.ImgPath = "/images/userDefault.png";
            }
            ///////////////////////////////////////////////////////////////////////
            //  FIN gestion image
            ///////////////////////////////////////////////////////////////////////
            ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;
            //pour la drop down list des roles 
            var listeRoles = _context.Roles.ToList();
            //trouve l id du role 
            var role = _context.Roles.Where(r => r.Name == utilisateur.Role).FirstOrDefault();
            var roleId = role.Id;
            ViewBag.listeRoles = new SelectList(listeRoles, "Id", "Name", roleId); //pour le DDL des roles

            return View(utilisateur);
        }
        /// <summary>
        /// Post edit /userId
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Utilisateur utilisateur)
        {

            if (ModelState.IsValid)
            {
                await _utilisateurManager.UpdateUtilisateurAdmin(utilisateur);
                ViewData["Message"] = "L'utilisateur a bien été modifié";
                return RedirectToAction(nameof(AdminController.Index), "Admin");
                //return RedirectToAction("Index", new RouteValueDictionary(new
                //{
                //    controller = "Admin",
                //    action = "Index"
                //}));
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            //      GESTION de la photo
            //////////////////////////////////////////////////////////////////////////////////////////
            if (utilisateur.UrlAvatarImage != null)
            {
                string img = utilisateur.UrlAvatarImage.ToString();
                ViewBag.ImgPath = img;
            }
            else
            {
                ViewBag.ImgPath = "/images/userDefault.png";
            }
            ///////////////////////////////////////////////////////////////////////
            //  FIN gestion image
            ///////////////////////////////////////////////////////////////////////
            ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;

            return View(utilisateur);
        }

        /// <summary>
        /// permet de supprimer un utilisateur grace à son userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string userId)
        {
            if (userId == null) // vérifie qu un id est bien passé en param
            {
                return NotFound();
            }
            //cherche le user dans la base 
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            if (utilisateur == null) //si on en trouve pas
            {
                return NotFound();
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            //      GESTION de la photo
            //////////////////////////////////////////////////////////////////////////////////////////
            if (utilisateur.UrlAvatarImage != null)
            {
                string img = utilisateur.UrlAvatarImage.ToString();
                ViewBag.ImgPath = img;
            }
            else
            {
                ViewBag.ImgPath = "/images/userDefault.png";
            }
            ///////////////////////////////////////////////////////////////////////
            //  FIN gestion image
            ///////////////////////////////////////////////////////////////////////
            return View(utilisateur);
        }
        /// <summary>
        /// permet de confirmer la suppression d un user grace a son userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        // POST: Utilisateur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            if (userId == null) // vérifie qu un id est bien passé en param
            {
                return NotFound();
            }
            //cherche le user dans la base 
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            if (utilisateur == null) //si on en trouve pas
            {
                return NotFound();
            }
            if (await _utilisateurManager.RemoveUtilisateur(userId))
            {
                ViewBag.Message = "L'utilisateur a bien été supprimé";
                //redirection vers l accueil
                return RedirectToAction("Index", new RouteValueDictionary(new
                {
                    controller = "Admin",
                    action = "Index"
                }));
            }
            else
            {
                ViewBag.error = "La suppression a rencontré un problème.";
                return View();
            }

        }

        /// <summary>
        /// creer Get un utilisateur
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            //pour le DDL des roles
            var listeRoles = _context.Roles.ToList();
            ViewBag.listeRoles = new SelectList(listeRoles, "Id", "Name");

            return View();
        }
         /// <summary>
        /// POST/CREATE
        /// ajoute un utilisation en mode admin
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <param name="mdp"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Utilisateur utilisateur, string mdp)
        {
            //vérification pour savoir si le pseudo est libre ou pas 
            if (await _utilisateurManager.PseudoExist(utilisateur.Pseudo)) //si pas libre on renvoie le formulaire
            {
                ViewBag.error = "Ce pseudo n'est pas disponible.";
                //pour le DDL des roles
                var listeRoles = _context.Roles.ToList();
                ViewBag.listeRoles = new SelectList(listeRoles, "Id", "Name");
                return View(utilisateur);
            }
            //vérification que le mail n est pas deja pris par qlq un 
            if (await _utilisateurManager.EmailExist(utilisateur.Email))
            {
                ViewBag.errorEmail = "Ce mail est déjà pris par un utilisateur.";
                //pour le DDL des roles
                var listeRoles = _context.Roles.ToList();
                ViewBag.listeRoles = new SelectList(listeRoles, "Id", "Name");
                return View(utilisateur);
            }
            //vérif qu un role a bien été selectionné 
            if (utilisateur.Role ==null)
            {
                ViewBag.errorRole = "Un rôle doit être choisi pour l'utilisateur.";
                //pour le DDL des roles
                var listeRoles = _context.Roles.ToList();
                ViewBag.listeRoles = new SelectList(listeRoles, "Id", "Name");
                return View(utilisateur);
            }
            //date de la creation 
            utilisateur.DateCreationUtilisateur = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View(utilisateur);
            }
            else
            {
                //créer un applicationUser 
                 var user = new ApplicationUser
                {
                    UserName = utilisateur.Email,
                    Email = utilisateur.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, mdp);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    //Pour le role
                    string role = _context.Roles.Where(r => r.Id == utilisateur.Role).FirstOrDefault().ToString();
                    var roleUtilisateur = await _userManager.AddToRoleAsync(user, role); // ajoute son role
                    await _context.SaveChangesAsync(); // save for role
                    _logger.LogInformation("Create role for user");

                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // a voir
                  //  await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");

                    //création d'un utilisateur "vide"
                    Utilisateur nouvel_utilisateur = new Utilisateur
                    {
                        ApplicationUserID = user.Id,
                        Pseudo = utilisateur.Pseudo,
                        DateCreationUtilisateur = DateTime.Now,
                        ProfilUtilisateurComplet = true,
                        ConfirmEmail = user.EmailConfirmed,
                        Email = user.Email,
                        DateDeNaissance = utilisateur.DateDeNaissance,
                        Role = role,
                        UrlAvatarImage = "/images/userDefault.png"
                    };
                   await _utilisateurManager.AddUtilisateurAdmin(nouvel_utilisateur); //ajoute le nouvel utilisateur

                 }
                ViewData["Message"] = "L'utilisateur a bien été ajouté";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        ///le compte en details de l utilisatation en GET
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            if (utilisateur == null)
            {
                return NotFound();
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            //      GESTION de la photo
            //////////////////////////////////////////////////////////////////////////////////////////
            if (utilisateur.UrlAvatarImage != null)
            {
                string img = utilisateur.UrlAvatarImage.ToString();
                ViewBag.ImgPath = img;
            }
            else
            {
                ViewBag.ImgPath = "/images/userDefault.png";
            }

            ///////////////////////////////////////////////////////////////////////
            //  FIN gestion image
            ///////////////////////////////////////////////////////////////////////

            return View(utilisateur);
        }
    }
}




