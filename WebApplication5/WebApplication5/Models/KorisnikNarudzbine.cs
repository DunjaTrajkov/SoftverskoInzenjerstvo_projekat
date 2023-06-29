using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class KorisnikNarudzbine
    {
        public Narudzbina Narudzbina { get; set; }
        public List<Katalog> Items { get; set; }
    }
}
