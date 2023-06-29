using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class SviEntiteti
    {
        public Korisnik Korisnik { get; set; }
        public List<Narudzbina> Narudzbine { get; set; }
        public List<Katalog> Katalozi { get; set; }
        public List<Proizvod> Proizvodi { get; set; }
    }
}
