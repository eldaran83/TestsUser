using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUsers.Models.Interfaces.Interfaces
    { 

   public  interface IUtilisateurManager
    {
        Task<IEnumerable<Utilisateur>> GetAllUtilisateursAsync();
        Task<Utilisateur> GetUtilisateurByIdAsync(string id);
        Task<Utilisateur> AddUtilisateur(ApplicationUser applicationUser,Utilisateur utilisateur);
        Task<Utilisateur> UpdateUtilisateur(Utilisateur utilisateur);
        Task<bool> PseudoExist(string pseudo);
        Task<bool> RemoveUtilisateur(string userId);

        // ZONE ADMIN
        Task<Utilisateur> AddUtilisateurAdmin(Utilisateur utilisateurAdmin);
        Task<Utilisateur> UpdateUtilisateurAdmin(Utilisateur utilisateurUpdateAdmin);
        Task<bool> EmailExist(string email);
    }
}
