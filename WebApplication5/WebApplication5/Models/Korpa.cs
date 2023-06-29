using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Korpa
    {
        public NarCena NarCena { get; set; }
        public List<KatalogCena> KataloziCene { get; set; }
    }
}
