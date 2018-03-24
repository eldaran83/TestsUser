using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestUsers.Data;
using TestUsers.Models.Interfaces.Interfaces;
using TestUsers.Models.Interfaces.Managers;

namespace TestUsers.Models.Interfaces.Managers
{ 
    public class UtilisateurManager : IUtilisateurManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilisateurManager(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Renvoi la liste de tous les utilisateurs
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Utilisateur>> GetAllUtilisateursAsync()
        {
            var utilisateurs = await _context.Utilisateurs
               .Include(u => u.ApplicationUser)
               .ToListAsync();

            return utilisateurs;
        }

        /// <summary>
        /// renvoi l utilisateur grace a son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Utilisateur> GetUtilisateurByIdAsync(string id)
        {
                Utilisateur user = await _context.Utilisateurs
                     .Include(m => m.ApplicationUser)
                     .FirstOrDefaultAsync(u => u.ApplicationUserID == id);
                return user;

        }

        /// <summary>
        /// ajoute un utilisateur en BDD
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public async Task<Utilisateur> AddUtilisateur(ApplicationUser applicationUser,Utilisateur utilisateur)
        {
            _context.Add(utilisateur); // ajoute l'utilisateur
            await _context.SaveChangesAsync();
           var roleUtilisateur= await _userManager.AddToRoleAsync(applicationUser, "Member"); // ajoute son role
            await _context.SaveChangesAsync();

            return utilisateur;
        }

        /// <summary>
        /// ajoute un utilisateur Admin 
        /// pour le moment c'est uniquement 1 seul admin en bdd 
        /// a revoir apres 
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <param name="utilisateurAdmin"></param>
        /// <returns></returns>
        public async Task<Utilisateur> AddUtilisateurAdmin(Utilisateur utilisateurAdmin)
        {
            _context.Add(utilisateurAdmin); // ajoute l'utilisateur
            await _context.SaveChangesAsync();
           // on ajoute pas de role car pour le moment l'admin eldaran83 a deja un role lors de l instanciation dans le starup
            await _context.SaveChangesAsync();

            return utilisateurAdmin;
        }

        /// <summary>
        /// met à jour un utilisateur en BDD
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public async Task<Utilisateur> UpdateUtilisateur(Utilisateur utilisateur)
        {
            try
            {
                _context.Update(utilisateur);
                await _context.SaveChangesAsync();
                return utilisateur;
            }
            catch (DbUpdateConcurrencyException ex )
            {
                Console.WriteLine("La MAJ de l'utilisateur a rencontré un problème :" +ex);
                return utilisateur;
            }
        }

        /// <summary>
        /// met à jour l utilisateur depuis la zone admin
        /// ce qui permet de tout modifier si on le veut 
        /// notammment le role de l utilisateur
        /// </summary>
        /// <param name="updateUtilisateurAdmin"></param>
        /// <returns></returns>
        public async Task<Utilisateur> UpdateUtilisateurAdmin (Utilisateur updateUtilisateurAdmin)
        {
            try
            {
                //je dois récupérer l id de l utilisateur a update , qui n'est pas le meme que celui connecté puisque je suis en mode admin 
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(updateUtilisateurAdmin.ApplicationUserID);
                
                //trouve l ancien role de l user
                var ancienRole = await _context.UserRoles.Where(u => u.UserId == applicationUser.Id).FirstOrDefaultAsync();
                var ancienRoleId = ancienRole.RoleId;
                var ancienRoleName = await _context.Roles.Where(r => r.Id == ancienRoleId).FirstOrDefaultAsync();
                var ancienRoleName2 = ancienRoleName.Name;

                //je dois trouver l id du role dans la table Role du nom du nouveau role pour l'utilisateur à update//
                var nouveauRole = await _context.Roles.Where(r => r.Id == updateUtilisateurAdmin.Role).FirstOrDefaultAsync();
                var roleIdAModifier = nouveauRole.Id;
                var roleNameAModifier = nouveauRole.Name;

                var roleDeLutilisateurAModifier = await _context.UserRoles.Where(u => u.UserId == applicationUser.Id).FirstOrDefaultAsync();

                  //supprimer le UserRole dans la role UserRole
                await _userManager.RemoveFromRoleAsync(applicationUser, ancienRoleName2); // efface son role présent
                //je dois changer dans la table userRole son role pour mettre celui qui est passé en param
                var roleUtilisateur = await _userManager.AddToRoleAsync(applicationUser, roleNameAModifier); // ajoute son role
                await _context.SaveChangesAsync();

                //trouve le nouveau role de l user
                var nouveauRoleUser = await _context.UserRoles.Where(u => u.UserId == applicationUser.Id).FirstOrDefaultAsync();
                var nouveauRoleId = nouveauRoleUser.RoleId;
                var nouveauRoleName = await _context.Roles.Where(r => r.Id == nouveauRoleId).FirstOrDefaultAsync();
                var nouveauRoleNameAPasserDansUtilisateur = nouveauRoleName.Name;

                //mise a jour du role dans utilisateur 
                updateUtilisateurAdmin.Role = nouveauRoleNameAPasserDansUtilisateur;
                //je met a jour tout le reste de l utilisateur 
                _context.Update(updateUtilisateurAdmin);
                await _context.SaveChangesAsync();
                return updateUtilisateurAdmin;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("La MAJ de l'utilisateur a rencontré un problème :" + ex);
                return updateUtilisateurAdmin;
            }
        }

        /// <summary>
        /// permet de supprimer un utilisateur et tout ce qui s en rapporte :
        /// fichiers,utilisateur, role, applicationUser
        /// a supprimer dans cet ordre car sinon ca pete 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUtilisateur (string userId)
        {
            //Le ApplicationUser est à supprimer en DERNIER !!
            try
            {
                //Supprime le dossier qui contient tous les fichiers de l'utilisateur  
                var dirPath = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot" + "/UserFiles/" + Convert.ToString(userId) + "/");
                try // si l utilisateur à un dossier image pour son avatar
                {
                    Directory.Delete(dirPath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("L'utilisateur n'a pas de dossier image "+ex);

                    //Supprime l'utilisateur
                    var utilisateur = await _context.Utilisateurs.SingleOrDefaultAsync(m => m.ApplicationUserID == userId);
                    _context.Utilisateurs.Remove(utilisateur);
                    await _context.SaveChangesAsync();

                    //Supprime le role du ApplicationUser
                    var roleApplicationUser = await _context.UserRoles.SingleOrDefaultAsync(m => m.UserId == userId);
                    _context.UserRoles.Remove(roleApplicationUser);
                    await _context.SaveChangesAsync();

                    //supprime le ApplicationUser 
                    var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == userId);
                    _context.Users.Remove(applicationUser);
                    await _context.SaveChangesAsync();

                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("La supression de l'utilisateur a rencontré un problème :"+ex);
                return false;
            }
        }

        /// <summary>
        /// vérifie en bdd si le speudo est libre
        /// </summary>
        /// <param name="pseudo"></param>
        /// <returns></returns>
        public async Task<bool> PseudoExist(string pseudo)
        {
            try
            {
               Utilisateur pseudoUtilisateur= await _context.Utilisateurs.Where(u =>u.Pseudo.ToUpper() == pseudo.ToUpper()).FirstOrDefaultAsync();
                if (pseudoUtilisateur != null)
                {
                    return true; //le pseudo existe en bdd
                }
                else
                {
                    return false; //le pseudo n 'existe pas en bdd
                }
             }
            catch (Exception ex)
            {
                 Console.WriteLine("L'accès a la requête s'est mal passée "+ex);
                return false;
            }
        }

        /// <summary>
        /// vérifie que le mail n est pas deja pris par un utilisateur 
        /// et donc qu il est unique
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> EmailExist(string email)
        {
            try
            {
                var emailUtilisateur = await _context.Utilisateurs.Where(u => u.Email.ToUpper() == email.ToUpper()).FirstOrDefaultAsync();
                if (emailUtilisateur != null)
                {
                    return true; //le pseudo existe en bdd
                }
                else
                {
                    return false; //le pseudo n 'existe pas en bdd
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("L'accès a la requête s'est mal passée " + ex);
                return false;
            }
        }
    }
}
