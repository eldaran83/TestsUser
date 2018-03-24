using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestUsers.Models.BO
{
    public class Aventure
    {
        [Key]
        public int AventureID { get; set; }

        [Display(Name = "Nom de l'aventure")]
        public string NomAventure { get; set; }
        [Display(Name = "Nombre de vote")]
        public decimal Vote { get; set; }
        [Display(Name = "Lien Url")]
        public string ImageUrl { get; set; }

        //nav
        public ICollection<Utilisateur> Joueurs { get; set; } //les utilisateurs qui ont joués a cette aventure
        public ICollection<MessageAventure> MessageAventures { get; set; } // les messages qui composent l aventure
    }
}
