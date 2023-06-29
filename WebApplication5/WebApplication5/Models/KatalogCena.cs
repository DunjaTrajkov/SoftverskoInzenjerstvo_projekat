using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class KatalogCena
    {
        public Katalog Katalog { get; set; }
        public List<Proizvod> Proizvodi { get; set; }
        public float CenaKatalog { get; set; }
    }
}
