using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    //tip proizvoda za dekoraciju je Tip=1
    //ovde je sustina samo pregled i kreiranje kataloga/korpa koja cuva sve izabrane proizvode(dekoracija || proizvod)
    public class DekoracijaController : Controller
    {
        private readonly modelContext _db; //database->db

        public DekoracijaController(modelContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}