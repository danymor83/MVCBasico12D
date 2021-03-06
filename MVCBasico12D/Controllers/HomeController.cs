using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCBasico12D.Models;

namespace MVCBasico12D.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(bool ? invalid)
        {
            //Esta Action puede o no recibir un bool invalid
            //En caso de recibir, disponibiliza el mensaje de error
            //Recibe el invalid en caso del Login resultar invalido
            if (invalid != null)
            {
                ViewBag.Erro = "display: inline; color:red;";
            }
            else
            {
                ViewBag.Erro = "display: none;";
            }
            return View();
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
    }
}
