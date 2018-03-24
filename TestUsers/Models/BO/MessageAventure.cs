using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestUsers.Models.BO
{
    public class MessageAventure
    {
        [Key]
        public int MessageAventureID { get; set; }

        [Display(Name = "Titre")]
        public string TitreMessage { get; set; }
        [Display(Name = "Contenu")]
        public string ContenuMessage { get; set; }
        [Display(Name = "Lien Url")]
        public string ImageUrl { get; set; }
        [Display(Name = "Choix 1")]
        public int? ChoixDirectionIdMessageNumero1 { get; set; }
        [Display(Name = "Choix 2")]
        public int? ChoixDirectionIdMessageNumero2 { get; set; }
        [Display(Name = "Choix 3")]
        public int? ChoixDirectionIdMessageNumero3 { get; set; }



        //nav et FK 
        public Aventure AventureID { get; set; } // un message appartient a une aventure
    }
}
