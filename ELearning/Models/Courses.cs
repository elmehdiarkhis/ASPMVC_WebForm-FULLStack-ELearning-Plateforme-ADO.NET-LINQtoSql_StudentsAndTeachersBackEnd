using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Courses
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string DateDebut { get; set; }
        public string Description { get; set; }
        public string photo { get; set; }
        public string NiveauEtude { get; set; }

        public float prix { get; set; }
        public string photoDetail1 { get; set; }
        public string photoDetail2 { get; set; }
        public string photoDetail3 { get; set; }
        public string photoDetail4 { get; set; }
        public int nbrEtoile { get; set; }
        public int nbrAchat { get; set; }
        public int DescriLong { get; set; }
        


    }
}
