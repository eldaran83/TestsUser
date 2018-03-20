using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUsers.Models.Repositories.IRepositories
{
   public  interface IRepositoryUtilisateur
    {
        Task<IEnumerable<Utilisateur>> GetAllUtilisateursAsync();
        Task<Utilisateur> GetUtilisateurByIdAsync(string id);
        Task<Utilisateur> AddUtilisateur(ApplicationUser applicationUser,Utilisateur utilisateur);
        Task<Utilisateur> UpdateUtilisateur(Utilisateur utilisateur);
        Task<bool> PseudoExist(string pseudo);
        Task<bool> RemoveUtilisateur(string userId);



    }
}
