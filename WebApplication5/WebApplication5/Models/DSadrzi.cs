using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class DSadrzi
    {
        public int Iddogadjaja { get; set; }
        public int Idkatalog { get; set; }

        public virtual Dogadjaj IddogadjajaNavigation { get; set; }
        public virtual Katalog IdkatalogNavigation { get; set; }
    }
}
