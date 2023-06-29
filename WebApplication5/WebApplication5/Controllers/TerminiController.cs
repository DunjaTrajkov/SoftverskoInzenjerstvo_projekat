using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    //ove terminCont ne koristimo moze da se brise
    public class TerminiController : Controller
    {
        private readonly modelContext _db; //database->db

        public TerminiController(modelContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}