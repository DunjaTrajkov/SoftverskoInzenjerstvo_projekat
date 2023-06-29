using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{           
    //tip proizvoda za ketering je Tip=0
    public class KeteringController : Controller
    {
        private readonly modelContext _db; //database->db

        public KeteringController(modelContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}