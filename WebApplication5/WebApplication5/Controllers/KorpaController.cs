using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;
using System.Web;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace WebApplication5.Controllers
{
   
    public class KorpaController : Controller, IAutorizacija
    {
        private readonly object kljuc = new object();
        private readonly modelContext _db; //database->db

        public KorpaController(modelContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        

        [HttpGet]
        public IActionResult PrikaziStanjeKorpe(int? page)
        {

            var a = HttpContext.Session.GetString("User");
            if (a == null)
            {
                return RedirectToAction("PrijaviSe", "User");
            }
            Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User")); //da bi vratio podatke koje ima

            int npage = page ?? 1;
            //int brojPrPoStr = 12;

            try
            {
                Korpa korpa = new Korpa();
                NarCena narCena = new NarCena();
                List<KatalogCena> KataloziCene = new List<KatalogCena>();
                var nar = _db.Narudzbina.Where(kor => kor.Idkorisnika == k.Idkorisnika && kor.StatusNarudzbine == 0).ToList().First();  //nalazim narudzbinu korisnika
                narCena.Narudzbina = nar;
                var ns = _db.NSadrzi.Where(nr => nr.Idnarudzbine == nar.Idnarudzbine).ToList(); //nalazim sve vrste u nsadrzi tabeli sa tom narudzbinom
                List<Katalog> katalozi = new List<Katalog>();
                foreach (var item in ns)
                {
                    katalozi.Add(_db.Katalog.Find(item.Idkatalog)); //nalazim sve kataloge
                }

                foreach (var kat in katalozi)
                {
                    int kolicinakataloga = ns.Where(id => id.Idkatalog == kat.Idkatalog).Select(kol => kol.Kolicina).ToList().First();
                    var proiz = _db.KSadrzi.Where(id => id.Idkatalog == kat.Idkatalog).ToList(); //nalazim sve proizvode u ksadrzi
                    List<Proizvod> proizvods = new List<Proizvod>();
                    foreach (var p in proiz)
                    {
                        proizvods.Add(_db.Proizvod.Find(p.Idproizvoda));
                    }
                    KatalogCena katalogCena = new KatalogCena();
                    katalogCena.Katalog = kat;
                    katalogCena.Proizvodi = proizvods;
                    float cenaKat = 0;
                    foreach (var i in proizvods)
                    {
                        int kolicinaProizvoda = proiz.Where(id => id.Idproizvoda == i.Idproizvoda).Select(kol => kol.KolicinaPro).ToList().First();
                        if (i.CenaDekoracije != null)
                        {
                            cenaKat += (float)i.CenaDekoracije * kolicinaProizvoda;
                        }
                        else
                        {
                            cenaKat += (float)i.CenaPorcije * kolicinaProizvoda;
                        }
                    }
                    katalogCena.CenaKatalog = cenaKat;
                    KataloziCene.Add(katalogCena);
                    narCena.CenaNar += katalogCena.CenaKatalog * kolicinakataloga;
                }
                korpa.KataloziCene = KataloziCene;
                korpa.NarCena = narCena;
                nar.UkupnaCena = (decimal) narCena.CenaNar;
                _db.SaveChanges();
                return View(korpa);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Greska sa pribavljanju menija, ponovite ponovo. Sa exceptionom " + e.Message);
                return RedirectToAction("Index", "Home");
            }



            
        }

        

        public async Task<IActionResult> IzbaciIzKorpe(int? id) //na narudzbinu sam mislio
        {
            //saljem id narudzbine i kad izbaci iz korpe znaci da izbacuje narudzbinu, tj da je brise iz sve

            if (id == null)
            {
                return BadRequest();
            }

            var p = await _db.Narudzbina.FindAsync(id);

            if (p == null)
            {
                return NotFound();
            }
            lock(kljuc)
            { 
                _db.Remove(p); //trebalo bi da bude kaskadno brisanje
                _db.SaveChangesAsync();
            }

            return RedirectToAction("PrikaziStanjeKorpe","Korpa");
        }

        #region Poruci

        public IActionResult Poruci(int? id)
        {
            //if (id == null) return BadRequest();

            var a = HttpContext.Session.GetString("User");
            if (a == null)
            {
                return RedirectToAction("PrijaviSe", "User");
            }
            Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));

            var nk = _db.Narudzbina.Include(nar => nar.NSadrzi).Select(items => new KorisnikNarudzbine { Narudzbina = items, Items = items.NSadrzi.Select(n => n.IdkatalogNavigation).ToList() }); //ovo je za korpu ali prob je sto nemamo ni menije :(

            
            //status 0 jer je trenutno kreirana a 1/ da je kliknuo korisnik na poruci
            var ts = nk.Where(lj => lj.Narudzbina.Idkorisnika == k.Idkorisnika && lj.Narudzbina.StatusNarudzbine == 0).ToList();


            foreach (var e in ts)
            {
                e.Narudzbina.StatusNarudzbine = 1; //da je porucena
            }
            _db.SaveChanges();


            return RedirectToAction("PrikaziStanjeKorpe", "Korpa");


        }

        #endregion
        #region Autorizacija

        public bool IspitajAdmina(Korisnik k)
        {
            if (k.Tip.Contains("admin"))
            {
                return true;
            }
            return false;
        }

        public bool IspitajMenadzera(Korisnik k)
        {
            if (k.Tip.Contains("menadzer") || k.Tip.Contains("menadžer"))
            {
                return true;
            }
            return false;
        }

        public bool IspitajPKorisnika(Korisnik k)
        {
            if (k.Tip.Contains("korisnik"))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}