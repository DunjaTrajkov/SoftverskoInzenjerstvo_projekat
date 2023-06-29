using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace WebApplication5.Controllers
{
    public class MenadzerController : Controller,IAutorizacija
    {

        private readonly modelContext _dbase; // dostupnost svemu
        private readonly object kljuc = new object(); 
        public MenadzerController(modelContext db)
        {
            _dbase = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Proizvodi(int? page)
        {

            int npage = page ?? 1;
            int brojPrPoStr = 20;
            try
            {
                var lista = _dbase.Proizvod.ToList().ToPagedList(npage, brojPrPoStr);
                return View(lista);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Greska sa pribavljanju proizvoda, ponovite ponovo. Sa exceptionom " + e.Message);
                return RedirectToAction("Index", "Home");
            }
            //ako ovo nece idete na redirect to action
        }


        [HttpGet]
        public IActionResult DodajProizvod()
        {

            return View();
        }




        [HttpPost]
        public IActionResult DodajProizvod(Proizvod proizv)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Podaci nisu ispravno uneti.");
                return View(proizv); //nastala je greska i ulazi samo ako je false odnostno nisu uneti validno podaci
            }
            lock(kljuc)
            { 
                 _dbase.AddAsync(proizv);
                 _dbase.SaveChangesAsync();
            }
            return RedirectToAction("Proizvodi", "Menadzer"); //trebalo bi da se ucita stranica sa svim proizvodima, bez tip

        }

        [HttpDelete]
        [Route("/ObrisiProizvod/{id}")]
        public IActionResult ObrisiProizvod(int? id)
        {

            if (id == null) //provera da ne bi doslo do excepiton
            {
                return NotFound();
            }

            var pro = _dbase.Proizvod.Where(p => p.Idproizvoda == id).FirstOrDefault();

            if (pro == null)
            {
                ModelState.AddModelError("", "Proizvod ne postoji u bazi.");
            }
            lock (kljuc)
            { 
                _dbase.Proizvod.Remove(pro);
                 _dbase.SaveChangesAsync();
            }
            return View();
        }

        [HttpGet]
        [Route("/PribaviProizvod/{id}")] //ne znam sto sam ga postavio
        public IActionResult PribaviProizvod(int? id)  //ukoliko ne posalje id on je null zato koristim ove nullabilne tipove
        {
            if (id == null)
            {
                return NotFound();
            }
            var pro = _dbase.Proizvod.Find(id);
            if (pro == null)
            {
                ModelState.AddModelError("", "Proizvod ne postoji u bazi.");
                return NotFound();
            }

            //stranica za izmenu proizvoda
            return View(pro);
        }

        [HttpPost] //moram da ispitam za edit da li ide post, u api apk se koristi put ovde nzm

        public IActionResult IzmeniProizvod(Proizvod pro)
        {
            if (pro != null)
            {
                if (ModelState.IsValid)
                {
                    //sve je validno 
                    lock(kljuc)
                    { 

                        _dbase.Proizvod.Update(pro);
                        _dbase.SaveChanges();
                    }
                    return RedirectToAction("Proizvodi", "Menadzer");
                }
            }
            return NotFound();
        }
        #region Radnik
        [HttpGet]
        public IActionResult ZaposliRadnika()
        {
            return View(); //otvara se forma sa poljima za unos radnika
        }
        [HttpPost]
        public IActionResult ZaposliRadnika(Radnik rad)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Molim ispravno popunite polja");
                return View(rad);
            }
            lock(kljuc)
            { 
            _dbase.Add(rad);
            _dbase.SaveChanges();
            }
            return View("ListaZaposljenih"); //gde pokazuje zaposljene
        }
        [HttpPost]
        public async Task<IActionResult> Otpusti(int? id)
        {
            if (id == null)
            {
                return NotFound("Bad request");
            }

            var pom = await _dbase.Radnik.FindAsync(id);

            if (pom == null)
            {
                ModelState.AddModelError("Ime", "Radnik nije zaposljen");
            }
            lock(kljuc)
            { 
                _dbase.Radnik.Remove(pom);
                 _dbase.SaveChanges();
            }
            return View("ListaZaposljenih");


        }
        [HttpGet]
        public IActionResult ListaRadnike(int? page)
        {
            int npage = page ?? 1;
            int brRadnika = 5;
            try {
                var radnici = _dbase.Radnik.ToList().ToPagedList(npage, brRadnika);


                return View(radnici);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
        [HttpGet]
        public IActionResult Detalji(int? id)
        {
            if (id == null)
            {
                return NotFound("Radnik ne postoji");
            }
            try
            {
                Radnik rad = _dbase.Radnik.Where(ra => ra.Idradnika == id).FirstOrDefault();
                return View(rad);
            }
            catch (Exception e)
            {
                return NotFound("Nastala greska sa bazom " + e.Message);
            }
        }
        [HttpGet]
        public IActionResult IzmeniStatus(int? id)
        {
            if (id == null)
            {
                return NotFound("Radnik ne postoji");
            }
            try
            {
                Radnik rad = _dbase.Radnik.Where(ra => ra.Idradnika == id).FirstOrDefault();
                return View(rad);
            }
            catch (Exception e)
            {
                return NotFound("Nastala greska sa bazom " + e.Message);
            }
        }
        [HttpPost]
        public IActionResult IzmeniStatus(Radnik r)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            lock(kljuc)
            { 
                _dbase.Radnik.Update(r);
                _dbase.SaveChanges();
            }
            return View("ListaRadnike");

        }


        #endregion



        public IActionResult DodajMeni()
        {      
                return View();
        }

        public IActionResult DodatMeni(Katalog k)
        {
            if (k != null)
            {
                k.Kreirao = 0;
                //var pomm = _dbase.Katalog.LastOrDefault();

                /* k.Idkatalog = pomm.Idkatalog + 1;*/ //vraca sledeci katalog za kreiranje ako dopustim bazi kreira novi random
                lock(kljuc)
                { 
                    _dbase.Katalog.Add(k);
                    _dbase.SaveChanges(); //kreiram katalog
                }

                //HttpContext.Session.SetString("MENI1", JsonConvert.SerializeObject(pom)); //saljem objekat u sesiju
                HttpContext.Session.SetString("Kat1", JsonConvert.SerializeObject(k));

                return RedirectToAction("Proizvodi", "Menadzer");
            }
            else
            {
                return BadRequest();
            }
        }

    
        public IActionResult DodajProizvodeUMeni(int? id)
        {

            if (id == null)
            {
                ModelState.AddModelError("Proiz","Greska! Niste oznacili proizvod koji zelite da dodate u meniju.");
            }

            //var kap = HttpContext.Session.GetString("MENI1");
            //KSadrzi ksad = JsonConvert.DeserializeObject<KSadrzi>(kap);
            var tf = HttpContext.Session.GetString("Kat1");
            if (tf != null)
            {
                Katalog k = JsonConvert.DeserializeObject<Katalog>(tf);
              
                if (k != null)
                {
                    Proizvod p = _dbase.Proizvod.Find(id);
                    if (p != null)
                    {
                        KSadrzi ksad = new KSadrzi();
                        var a = _dbase.KSadrzi.Where(o => o.Idproizvoda == id && o.Idkatalog==k.Idkatalog).FirstOrDefault(); //ukoliko ima ne dodajem

                        if(a==null)
                        { 
                            ksad.IdkatalogNavigation = k;
                            //ksad.Idkatalog = k.Idkatalog;
                            ksad.IdproizvodaNavigation = p;
                            ksad.Idproizvoda = p.Idproizvoda;
                            ksad.KolicinaPro = 1; //po jedan proizvod u meniju
                                                  //_dbase.KSadrzi.Attach(ksad);

                        
                            k.KSadrzi.Add(ksad);
                            _dbase.Attach(ksad);

                            _dbase.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Proizvodi", "Menadzer");
            }
            else
            {
                return RedirectToAction("DodajMeni", "Shop");
            }

        }

        #region RadSaNarudzbinama



        [HttpGet]
        public IActionResult PrikaziNarudzbine(int? page)
        {
            int npage = page ?? 1;
            int brojPrPoStr = 12;
       
            try
            {
                //var  t = _database.Katalog.Include(tp => tp.KSadrzi).Select(p => new KatalogSaProizvodima { Catalog = p, Products = p.KSadrzi.Select(q => q.IdproizvodaNavigation).ToList() }).ToList();

                var nk = _dbase.Narudzbina.Include(nar => nar.NSadrzi).Select(items => new KorisnikNarudzbine { Narudzbina = items, Items = items.NSadrzi.Select(n => n.IdkatalogNavigation).ToList() });

                var tp = nk.Where(ok => ok.Narudzbina.StatusNarudzbine == 1).ToList();


                var kip = tp.ToPagedList(npage, brojPrPoStr);



                return View(kip);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Greska sa pribavljanju menija, ponovite ponovo. Sa exceptionom " + e.Message);
                return RedirectToAction("Index", "Home");
            }
           
        }



        public IActionResult PotvrdiNarudzbinu(int? id)
        {

            if (id == null) return NotFound();

            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;
                if (IspitajMenadzera(k))
                {
                    //menadzer je

                    var n = _dbase.Narudzbina.Find(id);

                    n.StatusNarudzbine = 2; //potvrdjena je

                    var radnicko = _dbase.Radnik.Find(n.Idradnika);
                    radnicko.Slobodan = false;



                    _dbase.SaveChanges();

                    
                    return RedirectToAction("PrikaziNarudzbine", "Menadzer");
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            else
            {

                return RedirectToAction("PrijaviSe","User");
            }

        }

        public IActionResult OdbaciNarudzbinu(int? id)
        {

            if (id == null) return NotFound();

            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;
                if (IspitajMenadzera(k))
                {
                    //menadzer je
                    lock (kljuc)
                    {
                        var n = _dbase.Narudzbina.Find(id);
                        //brise je
                        n.StatusNarudzbine = 4; //potvrdjena je
                        var radnicko = _dbase.Radnik.Find(n.Idradnika);
                        radnicko.Slobodan = true;
                        var upit = _dbase.NSadrzi.Where(nr => nr.Idnarudzbine == id);
                        foreach (var ns in upit)
                        {
                            _dbase.NSadrzi.Remove(ns);
                        }

                        _dbase.Narudzbina.Remove(n);
                        //ovde pucaa nema cascade delete 
                        _dbase.SaveChanges();
                    }

                    return RedirectToAction("PrikaziNarudzbine", "Menadzer");
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            else
            {

                return RedirectToAction("PrijaviSe", "User");
            }

        }


        #endregion

        #region DodajDrugogMenadzera

        public IActionResult DodajMenadzera()
        {
            var kor1 = HttpContext.Session.GetString("User");

            if (kor1 != null)
            {
                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor1);
                if (IspitajMenadzera(user))
                {
                  
                    return RedirectToAction("Registrovanje", "User");
                }
            }
          
           return RedirectToAction("Index","Home");
            
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