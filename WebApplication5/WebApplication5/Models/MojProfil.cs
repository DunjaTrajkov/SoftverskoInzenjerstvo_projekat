using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class MojProfil
    {
        public Korisnik Korisnik { get; set; }
        public List<Narudzbina> Narudzbine { get; set; }
    }
}
