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
    public class AdminController : Controller, IAutorizacija
    {
        private readonly modelContext _dbase; //ovde samo admin ima pristup i on ce da dodaje sve i imace dostupnost svemu
        //sto je inace kod nas Menadzer
        public AdminController (modelContext db)
        {
            _dbase = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DodajMenadzera()
        {
            var kor1 = HttpContext.Session.GetString("User");

            if (kor1 != null)
            {
                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor1);
                if (IspitajAdmina(user))
                {
                  
                    return RedirectToAction("Registrovanje", "User");
                }
            }

            return RedirectToAction("Index", "Home");

        }
        public IActionResult DodajAdmina()
        {
            var kor1 = HttpContext.Session.GetString("User");

            if (kor1 != null)
            {
                Korisnik user = JsonConvert.DeserializeObject<Korisnik>(kor1);
                if (IspitajAdmina(user))
                {
                  
                    return RedirectToAction("RegistrujSe", "User");
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public IActionResult NastalaGreska()
        {
            //ViewBag.GRESKA =mess;
            return View();
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
    }
}