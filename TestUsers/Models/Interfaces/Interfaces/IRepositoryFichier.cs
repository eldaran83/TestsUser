using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUsers.Models.Repositories.IRepositories
{
    public interface IRepositoryFichier
    {
        string  SaveFichier(string webRoot, string userId, string nomDuDossier, IFormCollection form);
        void RemoveFichier(string path, string fichiersASupprimer);
    }
}
