using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestUsers.Data;
using TestUsers.Models.Repositories.IRepositories;

namespace TestUsers.Models.Repositories.Respositories
{
    public class UtilisateurRepository : IRepositoryUtilisateur
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilisateurRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
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
                Directory.Delete(dirPath, true);

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
    }
}
