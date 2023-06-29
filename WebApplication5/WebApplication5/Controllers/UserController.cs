using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using WebApplication5.Models;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;




//MORAO SAM USER JER SAM VEC KREIRAO JEDNOM KORISNIKA I NIJE HTEO DA RADI VIEW PA ZATO SAD JE DRUGI KOJI JE ZADUZEN ZA TO :D 


namespace WebApplication5.Controllers
{
    
    public class UserController : Controller,IAutorizacija
    {
        private readonly modelContext _db; //database->db
        private readonly object kljuc = new object();
        public UserController(modelContext db)
        {
            _db = db;
        }

       
        public IActionResult Index()
        {
            
             return View();
        }


        #region Profil
        //za logovanje/prijavljivanje
        public IActionResult PrijaviSe()
        {
            return View();
        }
      
         //register
        [HttpGet]
        public IActionResult Registrovanje()
         {
           
            return View();
        }


  
        [HttpPost]
        public IActionResult Registrovanje(Korisnik korisnik)
        {

            var json = HttpContext.Session.GetString("User");
             Korisnik user = null;

              if (json != null)
                {
                    user = JsonConvert.DeserializeObject<Korisnik>(json);
                }
            
            if (user == null)
                {
                    korisnik.Tip = "korisnik";
                }
                else
                  if (user != null)
                {
                    if (IspitajMenadzera(user))
                    {
                         korisnik.Tip = "menadzer";
                    }
                    else
                        if(IspitajAdmina(user))
                    {
                         korisnik.Tip = "admin";
                    }
                }
            
            //proveri sifre
            try
            {
                if (ModelState.IsValid /*&& IsteSu(korisnik.Sifra, korisnik.PotvrdaSifre)*/)
                {
                    //stanje je validno, odnosno svi parametri koji su navedeni u property modela su ispunjeni
                     lock(kljuc)
                        { 
                            if (PronadjiKorisnickoIme(korisnik.Username))
                            {
                                ModelState.AddModelError("Username", "Molim, unesite drugi username. Uneti je zauzet, mora biti jedinstven!");

                                return View(korisnik);
                            }

                        //korisnik.Tip = "korisnik";
                             _db.AddAsync(korisnik);
                             _db.SaveChangesAsync(); //sad je tek postovanu u bazu 
                            ModelState.Clear();
                        //da se uloguje nakon registracije
                        var us = _db.Korisnik.Find(korisnik.Idkorisnika);
                            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(us));
                        }

                    return RedirectToAction("Index", "Home"); //treba da ide na login page 

                }
                return View(korisnik); //vraca se na istu stranicu, znaci da je nastala neka greska ili neko polje nije validno uneto
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
      


     
        [HttpPost]

        public async Task<IActionResult> PrijaviSe(Korisnik model)
        {
             if (ModelState.IsValid)
            {
                var pom = await _db.Korisnik.Where(op =>  op.Username == model.Username && op.Sifra == model.Sifra).FirstOrDefaultAsync();
                if (pom == null)
                {
                    ModelState.AddModelError("Sifra", "Prijavljivanje nije uspelo.");
                    ViewBag.GRESKA = "Prijavljivanje nije uspelo";
                    //return RedirectToAction("NastalaGreska", "Admin"/* new { id = 1 }*/);
                    return View();
                }
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(pom));
              //  HttpContext.Session.SetString("Username", pom.Username); //key je USERNAME I NJEGA TREBA da propagiram
                return RedirectToAction("Ulogovani");
            }
            else
            {
                ModelState.AddModelError("Sifra", "Prijavljivanje nije uspelo");
                return View();
            }

            //ukoliko je prijavljen
        }


        public IActionResult OdjaviSe()
        {
            HttpContext.Session.Clear(); //odjava brise sve cookie
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Error() //ukoliko dodje do greske
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult MojProfil(int? id)
        {
            ViewBag.Profil = 0;
            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;

                var kin = new MojProfil();

                kin.Korisnik = k; //izmenio sam 
                //0 da je kreiran
                var nar = _db.Narudzbina.Where(i => i.Idkorisnika == k.Idkorisnika && i.StatusNarudzbine == 0).ToList();
                //var nar = _db.Narudzbina.Where(n => n.Idkorisnika == k.Idkorisnika).ToList();
                kin.Narudzbine = nar;

                return View(kin);
            }
            else
            {
                return RedirectToAction("PrijaviSe");
            }
              
        }

     
        [HttpGet] 
        public IActionResult VratiPrihvacene()
        {
            ViewBag.Profil = 0;
            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;

                var kin = new MojProfil();

                kin.Korisnik = k; //izmenio sam 
                //0 da je kreiran
                var nar = _db.Narudzbina.Where(i => i.Idkorisnika == k.Idkorisnika && i.StatusNarudzbine == 2).ToList();
                //var nar = _db.Narudzbina.Where(n => n.Idkorisnika == k.Idkorisnika).ToList();
                kin.Narudzbine = nar;

                return View("MojProfil",kin);
            }
            else
            {
                return RedirectToAction("PrijaviSe");
            }

        }

        [HttpGet]
        public IActionResult VratiPorucene(int? id)
        {
            ViewBag.Profil = 0;
            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;

                var kin = new MojProfil();

                kin.Korisnik = k; //izmenio sam 
                //0 da je kreiran
                var nar = _db.Narudzbina.Where(i => i.Idkorisnika == k.Idkorisnika && i.StatusNarudzbine == 1).ToList();
                //var nar = _db.Narudzbina.Where(n => n.Idkorisnika == k.Idkorisnika).ToList();
                kin.Narudzbine = nar;

                return View("MojProfil", kin);
            }
            else
            {
                return RedirectToAction("PrijaviSe");
            }

        }


        [HttpGet]
        public JsonResult DetaljiNarudzbine(int? id)
        {
            if (id == null)
            {
               // return BadRequest();
            }
            var p = _db.Narudzbina.Find(id);
            if (p == null)
            {
               // return NotFound();
            }
            var a = HttpContext.Session.GetString("User");
            if (a != null) //znaci da je prijavljen i da moze pristupiti svom profilu
            {
                Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                ViewBag.Username = k;
                //  var m = _db.Katalog.Where(o => o).Include(pl => pl.KSadrzi).Select(pr => new KatalogSaProizvodima { Katalog = pr, Proizvod = pr.KSadrzi.Select(q => q.IdproizvodaNavigation).Where(tf => tf.Tip == 0).ToList() });

                //var poma = _db.Narudzbina.Where(o => o.Idnarudzbine == id).Include(r => r.NSadrzi).Select(pt => new KorisnikNarudzbine { Narudzbina = pt, Items = pt.NSadrzi.Select(q => q.IdkatalogNavigation).ToList() }).ToList();
   
                ////pom.Katalog = _db.Katalog.Include(pl => pl.KSadrzi).ThenInclude(lf => lf.Select(r => r.IdkatalogNavigation).ToList()).ToList();
                //var m = _db.Katalog.Include(pl => pl.KSadrzi).Select(pr => new KatalogSaProizvodima { Katalog = pr, Proizvod = pr.KSadrzi.Select(q => q.IdproizvodaNavigation).Where(tf => tf.Tip == 0).ToList() }).ToList();

                //var sikaku = _db.Katalog.

                //u n_sadrzi posalji id narudzbine zatim includuj sa katalogom i kasnije vrati sve proizvode
                var klok = _db.NSadrzi.Where(i => i.Idnarudzbine == id).Include(j => j.IdkatalogNavigation).ThenInclude(pl => pl.KSadrzi).ToList();



                //var tf = _db.Narudzbina.Find(id);

                //var loknice = _db.Narudzbina.Where(i => i.Idnarudzbine == id).Include(j => j.NSadrzi).ToList();
              //  var t = loknice.Intersect(klok);
               
               // var presek = ParallelEnumerable.Intersect(, klok);


                NarKaPo po = new NarKaPo();
                // po.Narudzbina = tf;


        //    select Katalog.IDKatalog,  (SUM(KolicinaPro * ISNULL(CenaDekoracije, 0)) + SUM(KolicinaPro * ISNULL(CenaPorcije, 0))) from Katalog inner join K_Sadrzi on Katalog.IDKatalog = K_Sadrzi.IDKatalog inner
        //                                                                                                                                      join Proizvod
        //on K_Sadrzi.IDProizvoda = Proizvod.IDProizvoda
        //GROUP BY Katalog.IDKatalog

                return Json(klok);
            }

            else
            {
                //return RedirectToAction("PrijaviSe");
            }
            return null;
        }

        public IActionResult ProveriDatumeIUkloniisporucene()
        {

            return null;
        }



        //svaki put sa promenom stranice treba da se poziva
        public  IActionResult Ulogovani() //metoda za pozivanje da li je ulogovan
        {
            try
            {
                var a = HttpContext.Session.GetString("User"); //zbog proveru

                if (a != null) //znaci da postoji sesija
                {
                    Korisnik k = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("User"));
                    ViewBag.Username = k;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("PrijaviSe");
                }
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region InformacijeProf
        //za autorizaciju filter
        public IActionResult IzmeniInf()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult SacuvajIzmene(Korisnik model)
        {
            var json = HttpContext.Session.GetString("User");
            if (json == null) { return RedirectToAction("PrijaviSe","User"); }
            Korisnik user;
        
             user = JsonConvert.DeserializeObject<Korisnik>(json);
            var p = _db.Korisnik.Find(user.Idkorisnika);
            try
            {
                if (model.Ime != null)
                {
                    p.Ime = model.Ime;
                    user.Ime = model.Ime;
                }
                if (model.Prezime != null)
                {
                    p.Prezime = model.Prezime;
                    user.Prezime = model.Prezime;
                }
                if (model.Email != null)
                {
                    p.Email = model.Email;
                    user.Email = model.Email;
                }
                if (model.BrojTelefona != null)
                {
                    p.BrojTelefona = model.BrojTelefona;
                    user.BrojTelefona = model.BrojTelefona;
                }

                HttpContext.Session.Clear();
                var korisnik = _db.Korisnik.Find(user.Idkorisnika);
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(korisnik));
                _db.Korisnik.Update(p);
                _db.SaveChanges();
                return RedirectToAction("MojProfil", "User");
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region MetodeZaSifru
        [HttpGet]
        public IActionResult IzmeniSifru()
        {
            return View();
        }

        //[HttpPost]
      
        //public IActionResult IzmeniSifru(Korisnik k)
        //{
        //    try
        //    {
        //        if (k != null)
        //        {
        //            Korisnik prim = _db.Korisnik.Find(k.Idkorisnika);
        //            if (IsteSu(k.Sifra, k.PotvrdaSifre))
        //            {
        //                prim.Sifra = k.Sifra;
        //                _db.Korisnik.Update(prim); // 

        //            }
        //        }
        //        return View(k);
        //    }
        //    catch(Exception)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        
        [HttpGet]
        public IActionResult ZaboravljenaSifra()
        {
            return View();
        }

        public IActionResult ZaboravljenaSifra(string email, string username)
        {
            try
            {
                if (username == String.Empty || email == String.Empty)
                {
                    return NotFound();
                }

                var pom = _db.Korisnik.Where(op => op.Username == username && op.Email == email).SingleOrDefault();
                if (pom != null)
                {
                    //slanje mejla sa c# koda na mejl npr gmail 
                    ViewBag.Text = "Molimo proverite svoje postansko sanduce. Na mejlu koji ste uneli prilikom registracije, jer vam je tamo poslata sifra.";
                    //kod za slanje mejla gore unetom korisniku
                    SmtpClient client = new SmtpClient("smtp.gmail.com");
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential("dionisinformation@gmail.com", "dionis12345");

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("dionisinformation@gmail.com");
                    mailMessage.To.Add(pom.Email);
                    mailMessage.Body = "Sifra je: " + pom.Sifra;
                    mailMessage.Subject = "Odgovor na vas zahtev";
                    client.Send(mailMessage);

                    ViewBag.Obavestenje = "Proverite svoj mejl";


                    return RedirectToAction("PrijaviSe", "User");
                }
                return Error();
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region MetodeBezAkcije

        [NonAction]
        public bool PronadjiKorisnickoIme(string user) //korisnicko ime mora biti jedinstveno, jer ce se ono koristiti za logovanje !!
        {
            try
            {
                var pomUser = _db.Korisnik.Where(koris => koris.Username == user).FirstOrDefault(); //vraca korisnika ili null ako ne pronadje
                if (pomUser == null)
                {
                    return false;
                }

                return true;
            }
            catch(Exception)
            {
                RedirectToAction("Index", "Home");
                return true;
            }
        }

        [NonAction]

        public bool IsteSu(string pass, string conf)
        {
            if (pass == conf)
            {
                return true;
            }
            ModelState.AddModelError("Potvrda", "Sifre nisu iste!");
            return false;
            
        }


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


        #endregion
    }
}