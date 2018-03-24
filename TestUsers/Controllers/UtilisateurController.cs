using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestUsers.Data;
using TestUsers.Models;
using TestUsers.Models.Interfaces.Interfaces;
using TestUsers.Models.Repositories.IRepositories;
using TestUsers.Services;

namespace TestUsers.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class UtilisateurController : Controller
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
        public UtilisateurController(UserManager<ApplicationUser> userManager,
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

        //GET Utilisateur/Inscription/userId
        public async Task<IActionResult> Inscription(string userId)
        {
            //je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }
            if (userId == null)
            {
                return NotFound();
            }
            //gestion du fait que l utilisateur peut deja exister => renvoi sur la vue details
            Utilisateur lutilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            if (lutilisateur != null)
            {
                return RedirectToAction("MonCompte", new RouteValueDictionary(new
                {
                    controller = "Utilisateur",
                    action = "MonCompte",
                    Id = userId
                }));
            }
            // SINON 
            //Ajoute un utilisateur "vierge" avec son bon ID de facon certaine
            var user = await _userManager.FindByIdAsync(userId);
            //sinon je crée un utilisateur 
            Utilisateur nouvel_utilisateur = new Utilisateur
            {
                ApplicationUserID = user.Id,
                Pseudo = "",
                DateCreationUtilisateur = DateTime.Now,
                ProfilUtilisateurComplet = false,
                ConfirmEmail = user.EmailConfirmed,
                Email = user.Email,
                DateDeNaissance = DateTime.Now,
                Role = "Member", 
                UrlAvatarImage = "/images/userDefault.png"
            };
            await _utilisateurManager.AddUtilisateur(user,nouvel_utilisateur); //ajoute le nouvel utilisateur
           
            // recherche utilisateur avec l'id 
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);

            if (utilisateur == null)
            {
                return NotFound();
            }
            return View(utilisateur);
        }

 

        //POST Inscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscription(Utilisateur utilisateur)
        {
            //vérification pour savoir si le pseudo est libre ou pas 
            if (await _utilisateurManager.PseudoExist(utilisateur.Pseudo)) //si pas libre on renvoie le formulaire
            {
                ViewBag.error = "Ce pseudo n'est pas disponible.";
                 ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;
                return View(utilisateur);
            }

            utilisateur.DateCreationUtilisateur = DateTime.Now;
 
            if (ModelState.IsValid)
            {
                await _utilisateurManager.UpdateUtilisateur(utilisateur);
                return RedirectToAction("MonCompte", new RouteValueDictionary(new
                {
                    controller = "Utilisateur",
                    action = "MonCompte",
                    Id = utilisateur.ApplicationUserID
                }));
            }
              ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;
            return View(utilisateur);
        }

        /// <summary>
        ///le compte en details de l utilisatation en GET
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> MonCompte(string userId)
        {
            //je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);

            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MonCompte(Utilisateur utilisateur, List<IFormFile> AvatarImage)
        {
            utilisateur.DateCreationUtilisateur = DateTime.Now;
            

            if (ModelState.IsValid)
            {
                await _utilisateurManager.UpdateUtilisateur(utilisateur);
                return RedirectToAction("MonCompte", new RouteValueDictionary(new
                {
                    controller = "Utilisateur",
                    action = "MonCompte",
                    Id = utilisateur.ApplicationUserID
                }));
            }
            ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;
            return View(utilisateur);
        }



         [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            //je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }
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
            return View(utilisateur);
         }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Utilisateur utilisateur)
        {

                utilisateur.DateCreationUtilisateur = DateTime.Now;
              
                if (ModelState.IsValid)
                {
                    await _utilisateurManager.UpdateUtilisateur(utilisateur);
                    return RedirectToAction("MonCompte", new RouteValueDictionary(new
                    {
                        controller = "Utilisateur",
                        action = "MonCompte",
                        Id = utilisateur.ApplicationUserID
                    }));
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

        public async Task<IActionResult> Delete(string userId)
        {
            //je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }
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

        // POST: Utilisateur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            //je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }
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
                //redirection vers l accueil
                return RedirectToAction("Index", new RouteValueDictionary(new
                {
                    controller = "Home",
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
        /// verifie si l utilisateur exist ou pas 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool UtilisateurExists(string userId)
        {
            return _context.Utilisateurs.Any(e => e.ApplicationUserID == userId);
        }


        /// <summary>
        /// upload image
        /// pour le moment il ne gère pas dynamiquement le retour vers la page qui l'a appelé
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UploadImage (string userId)
        { 
           // je recupere la vraie identité de l user
            var ApplicationUser = _userManager.GetUserId(HttpContext.User);
            if (userId != ApplicationUser)
            {
                userId = ApplicationUser;
            }
            var utilisateur = await _context.Utilisateurs.SingleOrDefaultAsync(m => m.ApplicationUserID == userId);
            if (utilisateur == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserID"] = utilisateur.ApplicationUserID;
            return View(utilisateur);
        }

        /// <summary>
        /// upload image
        /// pour le moment ne gère pas dynamiquement le retour vers la page qui l'a appelé
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormCollection form)
        {
             //verifie qu un fichier est bien passé en param et qu'il y a qlq chose dedans
            if (form.Files == null || form.Files[0].Length == 0)
                return RedirectToAction("Edit", new { userId = Convert.ToString(form["ApplicationUserID"]) });

            string webRoot = _env.WebRootPath; // récupère l environnement
            string userId = Convert.ToString(form["userId"]); // sert à la personnalisation du dossier pour l utilisateur
            string nomDuDossier = "/Avatar/"; // variable qui sert à nommer le dossier dans lequel le fichier sera ajouté, ICI c est le dossier avatar

            //Comme l utilisateur ne peut avoir qu'un seul avatar, on vérifie avant d'ajouter un fichier
            //que le dossier n'a pas d autre image en supprimant tous les fichiers qui pourraient s y trouver
            try
            {
                var sourceDir = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot" + "/UserFiles/" + userId + nomDuDossier);

                string[] listeAvatar = Directory.GetFiles(sourceDir);
                // Copy picture files.          
                foreach (string f in listeAvatar)
                {
                    // Remove path from the file name.
                    string fName = f.Substring(sourceDir.Length);
                    _fichierRepository.RemoveFichier(sourceDir, fName);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            //ajoute le fichier 
            var avatarURl = _fichierRepository.SaveFichier(webRoot, userId, nomDuDossier, form);
            //cherche le user dans la base 
            var utilisateur = await _utilisateurManager.GetUtilisateurByIdAsync(userId);
            utilisateur.UrlAvatarImage = avatarURl;
            await _utilisateurManager.UpdateUtilisateur(utilisateur);
           
            //renvoi vers le edit 
            return RedirectToAction("Edit", new { userId = Convert.ToString(form["ApplicationUserID"]) });
        }
 

    }



}
