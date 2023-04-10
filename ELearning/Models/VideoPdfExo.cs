using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class VideoPdfExo
    {
        public List<VideosTable> lstVideoTable  { get; set; }
        public List<PDF> lstPdfTable { get; set; }
        public List<Exercice> lstExoTable { get; set; }
    }
}