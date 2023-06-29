using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class KSadrzi
    {
        public int KolicinaPro { get; set; }
        public int Idkatalog { get; set; }
        public int Idproizvoda { get; set; }

        public virtual Katalog IdkatalogNavigation { get; set; }
        public virtual Proizvod IdproizvodaNavigation { get; set; }
    }
}
