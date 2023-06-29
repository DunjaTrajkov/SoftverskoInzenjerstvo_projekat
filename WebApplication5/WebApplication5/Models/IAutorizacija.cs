using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    interface IAutorizacija
    {

        bool IspitajAdmina(Korisnik k);

        bool IspitajMenadzera(Korisnik k);

        bool IspitajPKorisnika(Korisnik k);
      
    }
}
