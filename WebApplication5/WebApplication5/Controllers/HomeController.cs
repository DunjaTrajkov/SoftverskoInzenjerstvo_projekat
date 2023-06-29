using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class HomeController : Controller, IAutorizacija
    {
        private readonly modelContext _database;
        public HomeController(modelContext _db)
        {
            _database = _db;
        }
        public IActionResult Index()
        {
            PocetniPodaci p = _database.PocetniPodaci.FirstOrDefault(); //try catch

        
            return View(p);
             
        }


       
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region IzmenePocetne
        [HttpPost]
        public IActionResult Izmeni(PocetniPodaci p)
        {
            if (p == null)
            {
                return NotFound();
            }

            PocetniPodaci t = _database.PocetniPodaci.FirstOrDefault();

            if (t == null)
            {
                return NotFound();
            }

            if (p.InformacijeODostavi != null)
            {
                t.InformacijeODostavi = p.InformacijeODostavi;
            }
            else
                if (p.ONama != null)
            {
                t.ONama = p.ONama;
            }
            else
                if (p.PolitikaPrivatnost != null)
            {
                t.PolitikaPrivatnost = p.PolitikaPrivatnost;
            }
            else
                if (p.UsloviKoriscenja != null)
            {
                t.UsloviKoriscenja = p.UsloviKoriscenja;
            }
            else
                if (p.KorisnickaUsluga != null)
            {
                t.KorisnickaUsluga = p.KorisnickaUsluga;
            }
            
            _database.PocetniPodaci.UpdateRange(t);
            _database.SaveChanges();

            return RedirectToAction("Index","Home");

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
