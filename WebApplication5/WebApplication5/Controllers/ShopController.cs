using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Web;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace WebApplication5.Controllers
{
    public class ShopController : Controller, IAutorizacija
    {
        private readonly modelContext _database;
        private readonly object kljuc = new object();
        public static short Strana { get; set; }
        public static short tipS { get; set; }
        public ShopController(modelContext db)
        {
            _database = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region MetodeZaKeteringIDek




        [HttpGet]
        public IActionResult Prikaz() //vraca sve
        {
            return View();
        }

        [HttpGet]
        public IActionResult KreirajMeniPoIzboru(int? page)
        {
            int npage = page ?? 1;

            int brojProizvoda = 9; //toliko mozemo kasnije da dodamo da sam korisnik podesi koliko hoce za sad je hardcoding
            try
            {
                var hrana = _database.Proizvod.Where(op => op.Tip == 0).ToList().ToPagedList(npage, brojProizvoda);

                ViewBag.ID = 0;
                Strana = 2;
                tipS = 1;
                return View("Prikaz", hrana);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }


        }
        [HttpGet]
        public IActionResult KreirajKatalogPoIzboru(int? page)
        {
            ViewBag.ID = 1;
            Strana = 2;
            tipS = 2;
            int npage = page ?? 1;
            int brojProizvoda = 9;
            List<Proizvod> dekoracija;
            try
            {
                dekoracija = _database.Proizvod.Where(op => op.Tip == 1).ToList();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            return View("Prikaz", dekoracija.ToPagedList(npage, brojProizvoda));
        }
        [HttpGet]
        public IActionResult SviProizvodi(int? page)
        {
            int npage = page ?? 1;
            int brojProizvoda = 18;
            ViewBag.ID = 2; //za sve proizvode

            try
            {
                var svi = _database.Proizvod.ToList().ToPagedList(npage, brojProizvoda);
                return View("Prikaz", svi);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }
        #endregion

        #region Termini
        //ovu metodu ne koristimo moze da se brise
        public IActionResult Termini() //za lociranje
        {
            return View();
        }

        public IActionResult DodajTermin()
        {

            return View();
        }



        public async Task<IActionResult> DodajTermine(Narudzbina nar)
        {
            if (nar == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("AdresaIsporuke", "Prazno polje adresa");
                return RedirectToAction("PrikaziTermine", "Shop");
            }
            //ispitam samo delove koje se proveravaju 
            //datum narucivanja i vreme naruc
            Strana = tipS = 0;

            //mora da se ispita korisnik da li je ulogovan na sistem!!!!

            //javlja mi gresku za strane kljuceve pa cu morati ovde da dodam i korisnika i radnika za datu narudzbinu a ne na kraju u korpu da bi 
            //ovo proslo

            var kor = HttpContext.Session.GetString("User");

            if (kor != null)
            {
                nar.DatumNarucivanja = DateTime.Now;

                if (DateTime.Compare(nar.DatumNarucivanja, nar.DatumIsporuke) > 0)
                {
                    return RedirectToAction("PrikaziTermine", "Shop");
                }

                
                var pom = await _database.Narudzbina.Where(o => o.DatumNarucivanja == nar.DatumNarucivanja && o.VremeIsporuke == nar.VremeIsporuke).FirstOrDefaultAsync();
                nar.StatusNarudzbine = 0; //svaki put kad se dodaje termin ona je u stanju kreirana

                //var pom = _database.Narudzbina.Find(nar);
                if (pom != null)
                {
                    //znaci da postoji zakazan term u to vreme i ne moze da se doda, salje se obavestenje!
                }

                HttpContext.Session.SetString("Nar5", JsonConvert.SerializeObject(nar)); //da bi omogucio visak klijenta da kreiraju
                
                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor);



                Radnik jadnicak = _database.Radnik.Where(p => p.Slobodan == true).FirstOrDefault();

                if (jadnicak == null)
                {
                    //Vraca View ili obavestenje da narudzbinu nije moguce izvrsiti jer nemamo slobodne radnike u tom
                    //danu i da proba sledeceg dana
                    return NotFound();
                }
                nar.Idkorisnika = user.Idkorisnika;
                nar.IdkorisnikaNavigation = user;
                nar.StatusNarudzbine = 0; //da je kreirana

                user.Narudzbina.Add(nar);

                nar.Idradnika = jadnicak.Idradnika;
                nar.IdradnikaNavigation = jadnicak;
                jadnicak.Slobodan = false;
                await _database.SaveChangesAsync(); //za radnika

                //ovde mi nesto sumnjivo
                //treba da se izvrsi provera ali ne pada mi nista koja moze biti



                _database.Narudzbina.Attach(nar);
                await _database.SaveChangesAsync();
                //ne saljem po sesiju iz razloga sto u narudzbinu imam korisnika koji je kreirao datu narudzbinu

                //  HttpContext.Session.SetString("Nar", JsonConvert.SerializeObject(user.Narudzbina.Last()));
                // 
                //sad bi trebalo da se ucita stranica sa prikazom da je dodat termin
                //ovo ostavljam na vama
                return RedirectToAction("PrikaziTermine", "Shop");
            }
            else
            {
                return RedirectToAction("PrijaviSe", "User");
            }

        }



        public IActionResult UkloniTermin(int? id)
        {
            var item = _database.Narudzbina.Find(id);
            if (item == null || id == null)
            {
                ModelState.AddModelError("Nardz", "Vrednost nije validna. Pokušajte ponovo kasnije.");
            }
            lock(kljuc)
            {
                var upit = _database.NSadrzi.Where(nr => nr.Idnarudzbine == id);
                foreach (var ns in upit)
                {
                    _database.NSadrzi.Remove(ns);
                }
                _database.Narudzbina.Remove(item);
            _database.SaveChanges();
            }
            return RedirectToAction("PrikaziTermine", "Shop");
        }

        public IActionResult PrikaziTermine()
        {
            List<Narudzbina> nar = _database.Narudzbina.ToList();
            return View(nar);
        }
        #endregion

        #region MeniC
        [HttpGet]
        public IActionResult PredlazemoMeni(int? page) //lista menije 
        {
            //poziva stranicu koja prikazuje nase menije


            int npage = page ?? 1;
            int brojPrPoStr = 12;
            Strana = 1;
            tipS = 1;
            try
            {
                //var  t = _database.Katalog.Include(tp => tp.KSadrzi).Select(p => new KatalogSaProizvodima { Catalog = p, Products = p.KSadrzi.Select(q => q.IdproizvodaNavigation).ToList() }).ToList();
                var m = _database.Katalog.Where(o => o.Kreirao == 0).Include(pl => pl.KSadrzi).Select(pr => new KatalogSaProizvodima { Katalog = pr, Proizvod = pr.KSadrzi.Select(q => q.IdproizvodaNavigation).Where(id => id.Tip == 0).ToList() });
                List<KatalogSaProizvodima> rez = new List<KatalogSaProizvodima>();
                foreach (var kp in m)
                {
                    if (kp.Proizvod.Count > 0)
                        rez.Add(kp);
                }

                var kip = rez.ToPagedList(npage, brojPrPoStr);



                return View(kip);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Greska sa pribavljanju menija, ponovite ponovo. Sa exceptionom " + e.Message);
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region KatalogD
        [HttpGet]
        public IActionResult PredlazemoKatalog(int? page) //lista katalog
        {
            //prikazuje  nase kataloge za dekoraciju

            int npage = page ?? 1;
            int brojPrPoStr = 12;
            Strana = 1;
            tipS = 2;
            try
            {
                //var  t = _database.Katalog.Include(tp => tp.KSadrzi).Select(p => new KatalogSaProizvodima { Catalog = p, Products = p.KSadrzi.Select(q => q.IdproizvodaNavigation).ToList() }).ToList();
                var m = _database.Katalog.Where(o => o.Kreirao == 0).Include(pl => pl.KSadrzi).Select(pr => new KatalogSaProizvodima { Katalog = pr, Proizvod = pr.KSadrzi.Select(q => q.IdproizvodaNavigation).Where(id => id.Tip == 1).ToList() });
                List<KatalogSaProizvodima> rez = new List<KatalogSaProizvodima>();
                foreach (var kp in m)
                {
                    if (kp.Proizvod.Count > 0)
                        rez.Add(kp);
                }

                var kip = rez.ToPagedList(npage, brojPrPoStr);



                return View(kip);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Greska sa pribavljanju menija, ponovite ponovo. Sa exceptionom " + e.Message);
                return RedirectToAction("Index", "Home");
            }

        }
        

        //ova metoda nije potrebna ali neka je moz zatreba u toku
        public IActionResult KreirajNarudzbinu()
        {
            //korisnik kada ode da dodaje nesto za porudzbinu kreira narudzbinu
            //poziva se kad doda termin i time kreira novu narudzbinu
            //necu da ispitujem jer ovo ce imati samo korisnik i moracemo da imamo jednu fju ili NAS ATRIBUT AUTORIZACIJE
            //a i moze da se kreira kada se popune vrednosti
            // Narudzbina nar = new Narudzbina(); 
            var t = HttpContext.Session.GetString("Nar");
            if (t != null)
            {
                Narudzbina nar = JsonConvert.DeserializeObject<Narudzbina>(t);

                var kor = HttpContext.Session.GetString("User");

                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor);
                //  Korisnik misko = _database.Korisnik.Find(user);
                user.Narudzbina.Add(nar);

                nar.Idkorisnika = user.Idkorisnika;
                nar.IdkorisnikaNavigation = user;
                nar.StatusNarudzbine = 0; //da je kreirana
                _database.SaveChanges();


                _database.Narudzbina.Add(nar); //gde se pre unose termini
                _database.SaveChanges();
                return RedirectToAction("DodajItem", "Shop"); //da svaki dodati elm kreira nsadrzi
            }
            else
            {
                return BadRequest();
            }
        }



        #endregion



        public IActionResult UkloniMeni(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Katalog men = _database.Katalog.Find(id);
            if (men == null)
                return NotFound();
            lock(kljuc)
            { 
                _database.Remove(men);
                _database.SaveChanges();
            }
            return RedirectToAction("Index", "Home"); //koristim cisto da ima negde da vrati inace ovo koristi samo menadzer

        }



        //poziva se kada dodaje predlozene menije/kataloge
        public IActionResult DodajPredlozeni(int? id) //takodje se poziva jer ne znm da li ce biti meni/kat ili po izboru prriz
        {

            var kor = HttpContext.Session.GetString("User");

            Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor);



            if (id == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            //dodaje kataloge koje nudi menadzer


            var n = HttpContext.Session.GetString("Nar5");
            //ako imam nar5 znaci da je zakazan termin
            if (n != null)
            {
                Narudzbina narcis = JsonConvert.DeserializeObject<Narudzbina>(n);
                if (narcis != null)
                {

                    Narudzbina nar = _database.Narudzbina.Where(o => o.Idkorisnika == user.Idkorisnika).LastOrDefault();

                    if (nar == null)
                    {
                        return NotFound();
                    }

                    if(nar.Idkorisnika != user.Idkorisnika)
                    {
                        return BadRequest();
                    }
                  //  Narudzbina nar = _database.Narudzbina.Where(u => u.Idkorisnika == user.Idkorisnika).LastOrDefault();

                    if (nar != null)
                    {
                        List<NSadrzi> upit = _database.NSadrzi.Where(nr => nr.Idnarudzbine == nar.Idnarudzbine && nr.Idkatalog == id).ToList();
                        if(upit.Count!=0)
                        {
                            return View();
                        }

                        NSadrzi ns = new NSadrzi();



                        //vracam katalog
                        Katalog kat = _database.Katalog.Find(id);
                        if (kat == null)
                        {
                            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                        }
                      //  kat.Kreirao = 1; //da ga kreirao korisnik
                        ns.Idkatalog = kat.Idkatalog;
                        ns.IdkatalogNavigation = kat;
                        ns.Idnarudzbine = nar.Idnarudzbine;
                        ns.IdnarudzbineNavigation = nar;
                        ns.Kolicina = 1;
                        // misko.Narudzbina.Add(nar);

                        //ns.Kolicina++;
                        nar.NSadrzi.Add(ns);
                        //_database.NSadrzi.Add(ns);
                        _database.NSadrzi.Add(ns);
                        _database.SaveChanges();


                        //  int tip = (ViewBag.TipS == null)?0:ViewBag.Tips;//tip strane da li je dekoracija ili hrana //zbog redirekcije
                        //odavde sam
                        return null;
                    }
                    else
                    {
                        //gde god narudzbina iz sesije je null da se redirektuje na termin jer nam  je termin ulazna tacka za porudzbinu!!
                        return RedirectToAction("PrikaziTermine", "Shop");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            else
            {
                ViewBag.Porukica = "Morate prvo zakazati termin!";
                ModelState.AddModelError("Porukica", "Morate prvo zakazati termin!");
                return RedirectToAction("PrikaziTermine", "Shop");
            }

        }


        //dodavanje proizvoda u korpu
        public IActionResult DodajItem(int? id)
        {

            //na osnovu view baga mozda mogu da odredim sta je kliknuo, da li je na kreiraj po izboru ili predlozeni
            //1 predlozeni //2 kreiraj po izboru isto je i za ket i za dek
            // int smer =(ViewBag.Strana==null) ? 0: ViewBag.Strana;
            var kor = HttpContext.Session.GetString("User");
            if (kor == null) { return RedirectToAction("PrijaviSe", "User"); }
            Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor);


            if (IspitajPKorisnika(user))
            {

                if (Strana == 1)
                {
                    DodajPredlozeni(id);
                    if (tipS == 1)
                    {
                        return RedirectToAction("PredlazemoMeni", "Shop");
                    }
                    else
                        if (tipS == 2)
                    {
                        return RedirectToAction("PredlazemoKatalog", "Shop"); //problem sto vraca uvek na prvu str, resenje da se prosledi page al videcu
                    }

                    return RedirectToAction("Index", "Home");

                }
                else
                    if (Strana == 2)
                {
                    //salje se id proizvoda
                    //vraca se ovde
                    DodajKreiraniPoIzboru(id);

                    if (tipS == 1)
                    {
                        return RedirectToAction("KreirajMeniPoIzboru", "Shop");
                    }
                    else
                       if (tipS == 2)
                    {
                        return RedirectToAction("KreirajKatalogPoIzboru", "Shop"); //problem sto vraca uvek na prvu str, resenje da se prosledi page al videcu
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.AUT = "Nemate privilegije!";
                return RedirectToAction("Index", "Home");
            }
            //uvek ce se poziva
            //return NotFound();
        }

        public IActionResult DodajKreiraniPoIzboru(int? id)
        {
            
            if (PostojiKatalog()) //proverava da li imam kreirani katalog
            {
                DodajProizvodeUMeni(id);
            }
            else
            {
                KreirajMeni();
                DodajProizvodeUMeni(id);
            }

            return null;
            //poziva metode menadzera
            //return NotFound();
        }

        [NonAction]
        public bool PostojiKatalog()
        {
            var tf = HttpContext.Session.GetString("Kata");
            if(tf != null)
            {
                //znaci da nije proso kroz kreiraj meni
                return true;
            }
            
            return false;
        }


        //ovo je za korisnika
        public IActionResult KreirajMeni()
        {
            //treba da imenuje katalog i da se kreira kome se dodeljuje k_sadrzi m,y
            //zatim da se dodeli k_sadrzi referenca na listu proizvoda 
            //da se dodeli lista proizvoda
            //i to da se sacuva na bazi
            var kor = HttpContext.Session.GetString("User");
            //ukoliko nije ulogovan nista ne radi, redirectuj na prijavi se 
            if (kor != null)
            {
                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor);

                //i proveri da je korisnik a ne menadzer ili admin

                if (IspitajPKorisnika(user)) //ukoliko je on ulazi u kreiranje
                {

                    //var narcis = HttpContext.Session.GetString("Nar5");
                    //if (narcis == null)
                    //{
                    //    return RedirectToAction("DodajTermin","Shop");
                    //}

                    ////var t = JsonConvert.DeserializeObject<Narudzbina>(narcis);
                    ////if (t == null) return NotFound();

                    lock(kljuc)
                    { 

                        var nar = _database.Narudzbina.Where(o => o.Idkorisnika == user.Idkorisnika).LastOrDefault();
                        if (nar == null)
                        {
                            return RedirectToAction("DodajTermin", "Shop");
                        }

                        if (nar.Idkorisnika != user.Idkorisnika)
                        {
                            return BadRequest();
                        }

                      //  Narudzbina nar = _database.Narudzbina.Where(u => u.Idkorisnika == user.Idkorisnika && u.Idnarudzbine==t.Idnarudzbine).LastOrDefault();

                        if (nar != null)
                        {
                            NSadrzi ns = new NSadrzi();

                            //treba u ns da se stavi 1
                            ns.Kolicina = 1;
                            Katalog k = new Katalog();
                            


                            k.Naziv = "KorisnickiMeni"; //ovo ce ispravite jer ja u back radim cisto da postavi
                            k.Kreirao = 1; //da ga je korisnik kreirao
                                           //var pomm = _dbase.Katalog.LastOrDefault();
                            //HttpContext.Session.SetString("Kata", JsonConvert.SerializeObject(k));
                            /* k.Idkatalog = pomm.Idkatalog + 1;*/ //vraca sledeci katalog za kreiranje ako dopustim bazi kreira novi random
                            ns.Idkatalog = k.Idkatalog;
                            ns.IdkatalogNavigation = k;
                            ns.Idnarudzbine = nar.Idnarudzbine;
                            ns.IdnarudzbineNavigation = nar;
                            nar.NSadrzi.Add(ns);

                            _database.NSadrzi.Add(ns);
                            _database.Katalog.Add(k);

                            var tm = _database.Katalog.LastOrDefault(); //vraca poslednji katalog koji je kreiran

                            HttpContext.Session.SetString("Kata", JsonConvert.SerializeObject(tm));


                            _database.SaveChanges(); //kreiram katalog
                           
                            //HttpContext.Session.SetString("MENI1", JsonConvert.SerializeObject(pom)); //saljem objekat u sesiju


                            //ovde puca iz razloga sto je self referencing

                            //treba da se redirektuje onde gde je kliknuo za kreiraj po izboru msm na kat ili meni proizvode


                            return null; //vracam null iz razloga sto se kasnije to redirectuje u okviru fje dodajitem
                            //koristim je kao obicnu funkciju
                          //  return RedirectToAction("Proizvodi", "Menadzer"); //gde se prikazuje polje za unos naziva i onda odabir proizvoda
                        }
                        else
                        {
                            ModelState.AddModelError("Narudzbina", "Morate prvo da zakazete termin!");
                            return RedirectToAction("PrikaziTermine", "Shop");

                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("PrijaviSe","User");
            }
            return BadRequest();
        }

        public IActionResult DodajProizvodeUMeni(int? id)
        {
            //isto ispitivanje da je korisnik !! 

            if (id == null)
            {
                ModelState.AddModelError("Proiz", "Greska! Niste oznacili proizvod koji zelite da dodate u meniju.");
            }

            //var kap = HttpContext.Session.GetString("MENI1");
            //KSadrzi ksad = JsonConvert.DeserializeObject<KSadrzi>(kap);
           
            //var tf = HttpContext.Session.GetString("Kata");
            //if (tf == null)
            //{
            //    return BadRequest();
            //}

            //ovde je problem!!! za kreiraj po izboru
            lock(kljuc)
            { 
                Katalog k =  _database.Katalog.LastOrDefault();
                if (k == null) { return BadRequest(); }
                //prob
                //Katalog k = _database.Katalog.Find(kt.Idkatalog);
           


                if (k != null)
                {
                    Proizvod p = _database.Proizvod.Find(id);
                    

                    if (p != null)
                    {

                        //ovde proverim za proizvod da se ne javljaju duplikati
                        var s = _database.KSadrzi.Where(z => z.Idkatalog == k.Idkatalog && z.Idproizvoda == p.Idproizvoda).FirstOrDefault();
                        if (s != null) { return null; }

                       
                        KSadrzi ksad = new KSadrzi();
                        
                        ksad.IdkatalogNavigation = k;
                        //ksad.Idkatalog = k.Idkatalog;
                        ksad.IdproizvodaNavigation = p;
                        ksad.Idproizvoda = p.Idproizvoda;
                        ksad.KolicinaPro = 1; //po jedan proizvod u meniju
                                              //_dbase.KSadrzi.Attach(ksad);




                        k.KSadrzi.Add(ksad);
                        _database.Attach(ksad);

                        _database.SaveChanges();

                    }
                }
            }
            //drugaciji redirect ce biti

            return null;
         //   return RedirectToAction("Proizvodi", "Menadzer");

        }

        

     
        #region IzracunajKolicinuKataloga



        //saljem id kataloga za kog racunam ukupnu cenu
        public decimal IzracunajCenuIKolicinuZaKatalog(int id) //racuna za 1 katalog!!!
        {
            var m = _database.Katalog.Where(o => o.Kreirao == 0).Include(pl => pl.KSadrzi).Select(pr => new KatalogSaProizvodima { Katalog = pr, Proizvod = pr.KSadrzi.Select(q => q.IdproizvodaNavigation).Where(idd => idd.Tip == 0).ToList() });



            KatalogSaProizvodima pom = m.Include(o => o.Katalog).Where(p => p.Katalog.Idkatalog == id).FirstOrDefault();

            KSadrzi poma = _database.KSadrzi.Where(z => z.Idkatalog == id).FirstOrDefault();

            decimal vrednost = 0;
            decimal vr1,vr2;
            foreach (var el in pom.Proizvod)
            {

                vr1 = (el.CenaDekoracije.HasValue == true) ? el.CenaDekoracije.Value : 0;
                vr2 = (el.CenaPorcije.HasValue == true) ? el.CenaPorcije.Value : 0;



                vrednost += poma.KolicinaPro * vr1 + poma.KolicinaPro * vr2;
            }
            
            return vrednost;
        }

        public void IzracunajCenuZaNarudzbinu(int id) //salje se N_SAD.ID_NAr ili narudzbina svejedno
        {
           
            var nk = _database.Narudzbina.Where(i=>i.Idnarudzbine==id).Include(nar1 => nar1.NSadrzi).Select(items => new KorisnikNarudzbine { Narudzbina = items, Items = items.NSadrzi.Select(n => n.IdkatalogNavigation).ToList() });
            //vraca narudzbinu i listu kataloga

            KorisnikNarudzbine pom = nk.Where(o => o.Narudzbina.Idnarudzbine == id).FirstOrDefault(); //vraca jednu narudzbinu sa listu kataloga

            List<NSadrzi> nsad = _database.NSadrzi.Where(u => u.Idnarudzbine == pom.Narudzbina.Idnarudzbine).ToList();

            decimal rumba = 0;
            foreach (var a in nsad)
            {
                //prolazim za svaku vrednost kataloga moram da pomnozim sa onu cenu koju vrati za sve kataloge
                decimal sumaSvihKataloga = 0;
               
                foreach (var b in pom.Items)
                {
                    //lista kataloga za datu narudzbinu
                    sumaSvihKataloga += IzracunajCenuIKolicinuZaKatalog(b.Idkatalog);
                    
                }
                rumba += a.Kolicina * sumaSvihKataloga;

            }

            //e sad racunam ukupnu cenu
            Narudzbina nar = _database.Narudzbina.Find(id);


            nar.UkupnaCena = rumba;

            _database.SaveChanges();

        }



        #endregion

        #region Autorizacija

        public  bool IspitajAdmina(Korisnik k)
        {
            if (k.Tip.Contains("admin"))
            {
                return true;
            }
            return false;
        }

        public  bool IspitajMenadzera(Korisnik k)
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